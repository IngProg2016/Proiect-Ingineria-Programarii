(function () {
    'use strict';

    var OutOfRangeApp = angular.module('OutOfRangeApp')

    .component('app', {
        templateUrl: '/templates/masterpage/app.html',
        $routeConfig: [
            { path: '/', name: 'Home', component: 'homeCmp' },
            { path: '/register', name: 'Register', component: 'registerCmp' },
            { path: '/login', name: 'Login', component: 'loginCmp', data: { guestOnly: true } },
            { path: '/logout', name: 'Logout', component: 'logoutCmp' },
            { path: '/user/profile/:userId', name: 'User', component: 'userCmp', data: { requiresLogin: true } },
            { path: '/user/profile', name: 'CurrentUser', component: 'userCmp', data: { requiresLogin: true } },
            { path: '/admin/categories', name: 'AdminCategories', component: 'adminCatCmp', data: { roles: ['admin'] } },
            { path: '/questions', name: 'Questions', component: 'questionsCmp' },
            { path: '/questions/add', name: 'AddQuestion', component: 'addQuestionCmp', data: { requiresLogin: true } },
            { path: '/question/:id', name: 'ViewQuestion', component: 'viewQuestionCmp' },
            { path: '/question/:id/:scrollTo', name: 'ViewQuestionScrollTo', component: 'viewQuestionCmp' },
            { path: '/*any', name: 'NotFound', component: 'notFoundCmp' }
        ]
    })
    .component('headSection', {
        templateUrl: '/templates/masterpage/header.html',
        controller: ['authService', HeadCtrl]
    })
    .component('footerSection', {
        templateUrl: '/templates/masterpage/footer.html',
        controller: [FooterCtrl]
    })
    .component('homeCmp', {
        templateUrl: '/templates/front-page.html',
        controller: [HomeCtrl]
    })
    .component('notFoundCmp', {
        templateUrl: '/templates/404.html'
    })
    .component('registerCmp', {
        templateUrl: '/templates/auth/register.html',
        controller: ['authService', 'routeChangeService', RegisterCtrl],
        bindings: {
            $router: '<'
        }
    })
    .component('loginCmp', {
        templateUrl: '/templates/auth/login.html',
        controller: ['authService', 'routeChangeService', LoginCtrl],
        bindings: {
            $router: '<'
        }
    })
    .component('logoutCmp', {
        controller: ['authService', LogoutCtrl],
        bindings: {
            $router: '<'
        }
    })
    .component('adminCatCmp', {
        templateUrl: 'templates/admin/categories.html',
        controller: ['adminService', AdminCatCmp]
    })
    .component('userCmp', {
        templateUrl: 'templates/user/profile.html',
        controller: ['userService', UserCtrl]
    })
    .component('questionsCmp', {
        templateUrl: '/templates/questions/questionAll.html',
        controller: ['$q', 'qaService', QuestionsCtrl]
    })
    .component('addQuestionCmp', {
        templateUrl: '/templates/questions/questionAdd.html',
        controller: ['$scope', 'qaService', 'routeChangeService', AddQuestionCtrl],
        bindings: {
            $router: '<'
        }
    })
    .component('viewQuestionCmp', {
        templateUrl: '/templates/questions/questionView.html',
        controller: ['$q', '$interval', 'smoothScroll', 'authService', 'qaService', ViewQuestionCtrl],
        bindings: {
            $router: '<'
        }
    })
    ;

    function HeadCtrl(authService) {
        var $ctrl = this;

        var authInfo = authService.getAuthentificationInfo;

        this.isAuthenticated = function () {
            return authInfo().isAuth;
        };

        this.isAdmin = function () {
            return true;
        };

        this.userName = function () {
            return authInfo().userName;
        }

    }

    function FooterCtrl() { }

    function HomeCtrl() {
        var $ctrl = this;
    }

    function RegisterCtrl(authService, routeChangeService) {
        var $ctrl = this;

        this.user = {
            email: '',
            password: '',
            confirmPassword: ''
        };

        this.error = null;
        this.loading = false;

        this.register = function () {
            $ctrl.loading = true;
            authService.register($ctrl.user)
            .then(function (result) {
                var loginInstruction = $ctrl.$router.generate(['Login']);
                loginInstruction.component.routeData.data.returnInstruction = $ctrl.$router.parent._currentInstruction.component.routeData.data.returnInstruction;
                $ctrl.$router.navigateByInstruction(loginInstruction);
            })
            .catch(function (err) {
                $ctrl.loading = false;
                $ctrl.error = err;
            });
        }

        this.$routerOnActivate = function (toRoute, fromRoute) { routeChangeService.onChange(toRoute, fromRoute); }
    }

    function LoginCtrl(authService, routeChangeService) {

        var $ctrl = this;
        var authInfo = authService.getAuthentificationInfo;

        this.user = {
            username: '',
            password: ''
        };

        this.error = null;
        this.rememberMe = false;

        this.loading = false;

        this.login = function () {
            $ctrl.loading = true;
            authService.login($ctrl.user, $ctrl.rememberMe)
            .then(function (result) {
                routeChangeService.navigateToRedirectUrl($ctrl.$router.parent._currentInstruction.component);
            })
            .catch(function (err) {
                $ctrl.loading = false;
                $ctrl.error = err.error_description;
            });
        }

        this.$routerOnActivate = function (toRoute, fromRoute) { routeChangeService.onChange(toRoute, fromRoute); }

    }

    function LogoutCtrl(authService) {
        var $ctrl = this;

        this.$routerOnActivate = function (toRoute, fromRoute) {
            authService.logout();
            if (fromRoute && !fromRoute.routeData.data.requiresLogin)
                $ctrl.$router.navigateByUrl(fromRoute.urlPath + '?' + fromRoute.urlParams.join('&'));
            else
                $ctrl.$router.navigate(['Home']);
        }
    }

    function AdminCatCmp(adminService) {
        var $ctrl = this;

        (function () {
            updateCategories();
        })();

        function updateCategories() {
            adminService.getCategories()
            .then(function (categories) {
                $ctrl.categories = categories;
            });
        }

        this.saveCategory = function (category) {
            adminService.saveCategory(category)
            .then(function () { updateCategories(); })
            .catch(function () { updateCategories(); });
        };

        this.deleteCategory = function (category) {
            adminService.deleteCategory(category)
            .then(function () { updateCategories(); })
            .catch(function () { updateCategories(); });
        };

        this.addCategory = function (category) {
            adminService.addCategory(category)
            .then(function () { updateCategories(); })
            .catch(function () { updateCategories(); });
        }
    }

    function UserCtrl(userService) {
        var $ctrl = this;
        (function () {
            $ctrl.userProfile = userService.getProfileInfo();
        })();
    }

    function QuestionsCtrl($q, qaService) {
        var $ctrl = this;

        this.error = null;

        this.$routerOnActivate = function () {
            return $q(function (resolve, reject) {
                qaService.getQuestions().then(function (data) {
                    $ctrl.data = data || [];
                    resolve();
                }).catch(function (err) {
                    // accept the route change but put an error on the page
                    $ctrl.error = err;
                    resolve();
                });
            });
        }
    }

    function AddQuestionCtrl($scope, qaService, routeChangeService) {
        var $ctrl = this;

        (function () {
            qaService.getCategories()
            .then(function (categories) {
                $ctrl.categories = categories;
            });

        })();

        this.tinymceOptions = {
            plugins: 'link image code',
            toolbar: 'undo redo | bold italic | alignleft aligncenter alignright | code'
        };

        this.question = {
            Category: '',
            Title: '',
            QuestionBody: '',
            Tags: '',
            TagString: ''
        };

        this.descriptionValidation = function (value, control, minlength) {
            debugger;
            var text = angular.element(value).text();
            text && (text = text.replace(' ', ''));

            if (text && text.length > 0)
                control.$setValidity('required', true, control);
            else
                control.$setValidity('required', false, control);

            if (text && text.length >= minlength)
                control.$setValidity('minlength', true, control);
            else
                control.$setValidity('minlength', false, control);
        }

        this.addQuestion = function (isValid) {
            if (!isValid) {
                $scope.$broadcast('show-errors-check-validity');
                return;
            }
            var taglist = $ctrl.question.TagString.split(/ |,/);
            $ctrl.question.Tags = []
            for (var tag of taglist) {
                $ctrl.question.Tags.push({ name: tag });
            }
            $ctrl.question.QuestionBody = $ctrl.question.QuestionBody.replace('\r', '').replace('\n', '</br>');

            qaService.addQuestion($ctrl.question)
            .then(function (result) {
                $ctrl.$router.navigate(['ViewQuestion', { id: result.ID }]);
            });
        };


        this.$routerOnActivate = function (toRoute, fromRoute) { routeChangeService.onChange(toRoute, fromRoute); }
    }

    function ViewQuestionCtrl($q, $interval, smoothScroll, authService, qaService) {
        var $ctrl = this;
        this.isAuth = authService.getAuthentificationInfo().isAuth;

        this.question = {};
        this.comment = {};
        this.error = null;
        this.scrollTo = null;

        this.prepareComment = function (parentID) {
            this.replyCommentID = parentID;
        }

        this.addComment = function () {
            $ctrl.comment.parentID = $ctrl.replyCommentID;
            qaService.addComment($ctrl.comment).then(function (_data) {
                _getQuestion($ctrl.question.ID);

                $ctrl.comment.commentBody = "";
            });
        }

        function _scrollToElement(scrollTo) {
            $ctrl.scrollTo = scrollTo;
        }

        this.$postLink = function () {
            var int = $interval(function () {
                if ($ctrl.scrollTo) {
                    $interval.cancel(int);
                    $interval(function () {
                        smoothScroll(document.getElementById($ctrl.scrollTo), { duration: 700, easing: 'easeOutQuad' });
                    }, 500, 1);
                }
            }, 100, 10);
        }

        this.$routerOnActivate = function (toRoute) {
            return $q(function (resolve, reject) {
                _getQuestion(toRoute.params.id).then(function () {
                    resolve();
                    _scrollToElement(toRoute.params.scrollTo);
                })
                .catch(function (err) {
                    $ctrl.error = err;
                    resolve();
                });
            });
        }

        function _getQuestion(questionId) {
            return qaService.viewQuestion(questionId).then(function (data) {
                $ctrl.question = data || {};
                return $q.resolve(data);
            });
        }

        this.plusQuestionVote = function () {
            //if ($ctrl.question.ScoreGiven)
            //    return;
            qaService.voteQuestion(1, $ctrl.question.ID)
            .then(function () { _getQuestion($ctrl.question.ID); });
        };

        this.minusQuestionVote = function () {
            //if ($ctrl.question.ScoreGiven)
            //    return;
            qaService.voteQuestion(-1, $ctrl.question.ID)
            .then(function () { _getQuestion($ctrl.question.ID); });
        };

        this.answer = {};

        this.addAnswer = function () {
            if (!$ctrl.answer.AnswerBody) {
                error = 'Cannot post a question without a body.';
                return;
            }
            $ctrl.answer.QuestionID = $ctrl.question.ID;
            $ctrl.answer.AnswerBody = $ctrl.answer.AnswerBody.replace('\r', '').replace('\n', '</br>');
            qaService.addAnswer($ctrl.answer).then(function (_data) {
                _getQuestion($ctrl.question.ID);

                $ctrl.answer.AnswerBody = "";
            });
        }

        this.plusAnswerVote = function (answer) {
            //if (answer.ScoreGiven)
            //    return;
            qaService.voteAnswer(1, answer.ID)
            .then(function () { _getQuestion($ctrl.question.ID); });
        };

        this.minusAnswerVote = function (answer) {
            //if (answer.ScoreGiven)
            //    return;
            qaService.voteAnswer(-1, answer.ID)
            .then(function () { _getQuestion($ctrl.question.ID); });
        };

        this.acceptAnswer = function (answer) {
            if (answer.Accepted)
                return;
            qaService.acceptAnswer(answer.ID)
            .then(function () { _getQuestion($ctrl.question.ID); });
        }

        this.plusCommentVote = function (comment) {
            //if (comment.ScoreGiven)
            //    return;
            qaService.voteComment(1, comment.ID)
            .then(function () { _getQuestion($ctrl.question.ID); });
        }

        this.minusCommentVote = function (comment) {
            //if (comment.ScoreGiven)
            //   return;
            qaService.voteComment(-1, comment.ID)
            .then(function () { _getQuestion($ctrl.question.ID); });
        }

        this.navigateWithReturn = function (routeLink, scrollTo) {
            var loginInstruction = $ctrl.$router.generate(routeLink);
            loginInstruction.component.routeData.data.returnInstruction = $ctrl.$router.generate(_getReturnLink(scrollTo));
            $ctrl.$router.navigateByInstruction(loginInstruction);
        }

        function _getReturnLink(scrollTo) {
            var link = ['ViewQuestion', { id: $ctrl.question.ID }];
            if (scrollTo) {
                link[0] = 'ViewQuestionScrollTo'
                link[1].scrollTo = scrollTo;
            }
            return link;
        }
    }

})();
