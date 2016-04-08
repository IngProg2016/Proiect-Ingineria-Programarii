(function () {
    'use strict';

    angular.module('OutOfRangeApp')
    .config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
        $routeProvider
            .when('/', {
                templateUrl: '/templates/front-page.html',
                controller: 'HomeCtrl'
            })
            .when('/register', {
                templateUrl: '/templates/Auth/register.html',
                controller: 'RegisterCtrl',
                guestOnly: true
            })
            .when('/login', {
                templateUrl: '/templates/Auth/login.html',
                controller: 'LoginCtrl',
                guestOnly: true
            })
            .when('/logout', {
                controller: 'LogoutCtrl',
                template: ''
            })
            .when('/user', {
                controller: 'UserCtrl',
                templateUrl: '/templates/user/user.html',
                requiresLogin: true
            })
            .when('/questions', {
                controller: 'QuestionsCtrl',
                templateUrl: '/templates/Questions/questionAll.html'
            })
            .when('/questions/add', {
                controller: 'QuestionAddCtrl',
                templateUrl: '/templates/Questions/questionAdd.html',
                requiresLogin: true
            })
            .when('/question/:id/:scrollTo', {
                controller: 'QuestionViewCtrl',
                templateUrl: '/templates/Questions/questionView.html',
            })
            .otherwise({
                templateUrl: '/templates/404.html'
            });
        $locationProvider.html5Mode(true);
    }]);
})();