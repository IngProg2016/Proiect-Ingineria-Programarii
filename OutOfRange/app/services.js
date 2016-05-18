(function () {
    'use strict';

    var OutOfRangeApp = angular.module('OutOfRangeApp')
    .service('routeChangeService', ['$rootRouter', 'authService', RouteChangeService])
    .service('authService', ['$q', '$resource', 'storageService', AuthService])
    .service('adminService', ['$q', '$resource', AdminService])
    .service('storageService', ['$localStorage', '$sessionStorage', StorageService])
    .service('authInterceptorService', ['$q', '$rootRouter', 'storageService', AuthInterceptorService])
    .service('qaService', ['$resource', QaService])
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
                console.warn('roles not implemented! (RouteChangeService -> onChange)');
                $svc.navigateToRedirectUrl(toRoute);
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

    function AuthService($q, $resource, storageService) {
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
        var Categories = $resource('/api/categories');

        this.getCategories = function () {
            return Categories.query().$promise;
        }

        this.saveCategory = function (category) {
            var _category = new Categories(category);
            _category.$save();
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

        this.auth.remove = function () {
            delete $localStorage.authData;
            delete $sessionStorage.authData;
        }
    }

    function QaService($resource) {
        var Questions = $resource('/api/questions/:id');
        var Answer = $resource('/api/answers');
        var Comment = $resource('/api/comments');
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

        this.addAnswer = function (answer) {
            return new Answer(answer).$save();
        }

        this.addComment = function (comment) {
            return new Comment(comment).$save();
        }
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