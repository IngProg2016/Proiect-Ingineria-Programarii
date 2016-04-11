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
            { path: '/user', name: 'User', component: 'userCmp', data: { requiresLogin: true } },
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
    .component('userCmp', {
        templateUrl: '',
        controller: ['authService', UserCtrl]
    })
    .component('questionsCmp', {
        templateUrl: '/templates/questions/questionAll.html',
        controller: ['$q', 'qaService', QuestionsCtrl]
    })
    .component('addQuestionCmp', {
        templateUrl: '/templates/questions/questionAdd.html',
        controller: ['qaService', 'routeChangeService', AddQuestionCtrl]
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

    function UserCtrl() { }

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

    function AddQuestionCtrl(qaService, routeChangeService) {
        var $ctrl = this;

        this.question = {
            Title: '',
            QuestionBody: '',
            Tags: '',
            TagString: ''
        };

        this.addQuestion = function () {
            var taglist = $ctrl.question.TagString.split(/ |,/);
            $ctrl.question.Tags = []
            for (var tag in taglist) {
                $ctrl.question.Tags.push({ name: taglist[tag] });
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

        this.prepareComment = function(parentID){
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

        this.answer = {};

        this.addAnswer = function () {
            $ctrl.answer.QuestionID = $ctrl.question.ID;
            $ctrl.answer.AnswerBody = $ctrl.answer.AnswerBody.replace('\r', '').replace('\n', '</br>');
            qaService.addAnswer($ctrl.answer).then(function (_data) {
                _getQuestion($ctrl.question.ID);

                $ctrl.answer.AnswerBody = "";
            });
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
