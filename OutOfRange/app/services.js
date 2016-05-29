(function () {
    'use strict';

    var OutOfRangeApp = angular.module('OutOfRangeApp')
    .service('routeChangeService', ['$rootRouter', 'authService', RouteChangeService])
    .service('authService', ['$q', '$resource', '$timeout', 'storageService', 'userService', AuthService])
    .service('userService', ['$resource', UserService])
    .service('adminService', ['$q', '$resource', AdminService])
    .service('storageService', ['$localStorage', '$sessionStorage', StorageService])
    .service('authInterceptorService', ['$q', '$rootRouter', 'storageService', AuthInterceptorService])
    .service('qaService', ['$resource', QaService])
    .service('searchService', ['$resource', SearchService])
    .config(['$httpProvider', function ($httpProvider) {
        $httpProvider.interceptors.push('authInterceptorService');
    }])
    ;

    function RouteChangeService($rootRouter, authService) {
        var $svc = this;

        this.onChange = function (toRoute, fromRoute) {
            if (toRoute.routeData.data.requiresLogin && !authService.getAuthentificationInfo().isAuth) {
                $rootRouter.recognize(toRoute.urlPath + '?' + toRoute.urlParams.join('&'))
                .then(function (result) {
                    var loginIntruction = $rootRouter.generate(['Login']);
                    loginIntruction.component.routeData.data.returnInstruction = result;
                    $rootRouter.navigateByInstruction(loginIntruction);
                });
            }

            if (toRoute.routeData.data.guestOnly && authService.getAuthentificationInfo().isAuth) {
                $svc.navigateToRedirectUrl(toRoute);
            }

            if (toRoute.routeData.data.roles) {
                var authInfo = authService.getAuthentificationInfo();
                if (!authInfo.isAuth || !authInfo.roles || authInfo.roles.indexOf('Moderator') == -1)
                    $rootRouter.navigate(['Home']);
            }
        }

        this.navigateToRedirectUrl = function (instruction) {
            if (instruction.routeData.data.returnInstruction) {
                $rootRouter.navigateByInstruction(instruction.routeData.data.returnInstruction);
                delete instruction.routeData.data.returnInstruction;
            } else {
                var returnLink = instruction.params.returnUrl ? JSON.parse(decodeURIComponent(instruction.params.returnUrl)) : ['Home'];
                $rootRouter.navigate(returnLink);
            }
        }
    }

    function AuthService($q, $resource, $timeout, storageService, userService) {
        var Register = $resource('/api/account/register');
        var Login = $resource('/token', null, {
            save: {
                method: 'POST',
                headers: { 'Accept': '*/*', 'Content-Type': 'application/x-www-form-urlencoded' },
                transformRequest: function (data, headersGetter) {
                    var str = [];
                    for (var d in data)
                        if (data.hasOwnProperty(d))
                            str.push(encodeURIComponent(d) + '=' + encodeURIComponent(data[d]));
                    return str.join('&');
                }
            }
        });

        this.register = function (user) {
            var _user = new Register(user);
            return _user.$save()
                .then(function (data) {
                    return $q.resolve(data);
                })
                .catch(function (err) {
                    console.error(err.data);
                    return $q.reject(err.data);
                });
        };

        this.login = function (user, rememberMe) {
            user.grant_type = 'password';
            var _user = new Login(user);
            return _user.$save()
                .then(function (data, headers) {
                    storageService.auth.set(rememberMe, {
                        isAuth: true,
                        userName: data.userName,
                        token: data.access_token,
                        _expiration: new Date(data['.expires'])
                    });

                    $timeout(function () {
                        userService.getCurrentUser()
                            .then(function (cUser) {
                                storageService.auth.update({
                                    isAuth: true,
                                    userName: data.userName,
                                    token: data.access_token,
                                    _expiration: new Date(data['.expires']),
                                    roles: cUser.roles
                                });
                            });
                    }, 500);

                    return $q.resolve({
                        isAuth: storageService.auth.get().isAuth,
                        userName: storageService.auth.get().userName,
                    });
                })
                .catch(function (err) {
                    console.error(err.data.error_description);
                    storageService.auth.remove();
                    return $q.reject(err.data);
                });
        };

        this.logout = function () {
            storageService.auth.remove();
        }

        this.getAuthentificationInfo = function () {
            return storageService.auth.get();
        }
    }

    function AdminService($q, $resource) {
        var Categories = $resource('/api/categories/:id', {}, {
            update: { method: 'PUT' }
        });

        this.getCategories = function () {
            return Categories.query().$promise;
        }

        this.saveCategory = function (category) {
            var _category = new Categories(category);
            return _category.$update({ id: _category.ID });
        }

        this.deleteCategory = function (category) {
            var _category = new Categories(category);
            return _category.$delete({ id: _category.ID });
        }

        this.addCategory = function (category) {
            var _category = new Categories(category);
            return _category.$save();
        }

    }

    function StorageService($localStorage, $sessionStorage) {
        var $svc = this;

        this.auth = {};

        this.auth.get = function () {
            var authData = $sessionStorage.authData || $localStorage.authData;

            if (authData && (new Date(authData._expiration) < Date.now()))
                $svc.auth.remove();

            if (authData) {
                authData._expiration = new Date(authData._expiration);
                return authData;
            }
            return { isAuth: false };
        }

        this.auth.set = function (rememberMe, data) {
            if (rememberMe)
                $localStorage.authData = data;
            else
                $sessionStorage.authData = data;
        }

        this.auth.update = function (data) {
            if ($localStorage.authData)
                $localStorage.authData = data;
            if ($sessionStorage.authData)
                $sessionStorage.authData = data;
        }

        this.auth.remove = function () {
            delete $localStorage.authData;
            delete $sessionStorage.authData;
        }
    }

    function QaService($resource) {
        var Questions = $resource('/api/questions/:id', {}, {
            update: { method: 'PUT' }
        });
        var Answer = $resource('/api/answers/:action/:id', {}, {
            update: { method: 'PUT' }
        });
        var Comment = $resource('/api/comments/:action');
        var Category = $resource('/api/categories');

        this.getCategories = function () {
            return Category.query().$promise;
        }

        this.getQuestions = function (filter) {
            return Questions.query(filter).$promise;
        }

        this.addQuestion = function (question) {
            return new Questions(question).$save();
        }

        this.viewQuestion = function (questionId) {
            return Questions.get({ id: questionId }).$promise;
        }

        this.voteQuestion = function (vote, questionId) {
            return new Questions({ score: vote, id: questionId }).$save({ id: 'addscore' });
        }

        this.addAnswer = function (answer) {
            return new Answer(answer).$save();
        }

        this.voteAnswer = function (vote, answerId) {
            return new Answer({ score: vote, id: answerId }).$save({ action: 'addscore' });
        }

        this.acceptAnswer = function (answerId) {
            return new Answer({ id: answerId }).$save({ id: answerId, action: 'accept' });
        }

        this.addComment = function (comment) {
            return new Comment(comment).$save();
        }

        this.voteComment = function (vote, commentId) {
            return new Comment({ score: vote, id: commentId }).$save({ action: 'addscore' });
        }

        this.updateQuestion = function (question) {
            return new Questions({ QuestionBody: question.QuestionBody }).$update({ id: question.ID });
        }

        this.updateAnswer = function (answer) {
            return new Answer({ AnswerBody: answer.AnswerBody }).$update({ id: answer.ID });
        }

        this.getValidAnswers = function () {
            return $resource('/api/answers/acceptedanswers').get().$promise;
        }

    }

    function UserService($resource) {
        var Profile = $resource('/api/user/profile/:id');
        var CurrentUser = $resource('/api/user/userdata');

        this.getProfileInfo = function (userId) {
            if (userId)
                return Profile.get({ id: userId }).$promise;
            return Profile.get().$promise;
        }

        this.getCurrentUser = function () {
            return new CurrentUser.get().$promise;
        }

        this.getUsers = $resource('api/users').query().$promise;
    }

    function SearchService($resource) {
        return $resource('/api/questions/search/:keywords');
    }

    function AuthInterceptorService($q, $rootRouter, storageService) {

        this.request = function (config) {
            config.headers = config.headers || {};

            if (config.url === '/token')
                return config;

            var authData = storageService.auth.get();
            if (authData) {
                config.headers.Authorization = 'Bearer ' + authData.token;
            }
            return config;
        };

        this.responseError = function (rejection) {
            if (rejection.status === 401) {
                $rootRouter;

                var loginIntruction = $rootRouter.generate(['Login']);
                loginIntruction.component.routeData.data.returnInstruction = $rootRouter._currentInstruction;
                $rootRouter.navigateByInstruction(loginIntruction);
            }
            return $q.reject(rejection);
        }
    }

})();