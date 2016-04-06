(function () {
    'use strict';

    var OutOfRangeApp = angular.module('OutOfRangeApp');

    OutOfRangeApp.factory('authService', ['$q', '$resource', '$localStorage', '$location', '$routeParams', function ($q, $resource, $localStorage, $location, $routeParams) {
        var Register = $resource('/api/account/register');
        var Login = $resource('/token', null, {
            save: {
                method: 'POST',
                headers: { 'Accept': '*/*', 'Content-Type': 'application/x-www-form-urlencoded' },
                transformRequest: function (data, headersGetter) {
                    var str = [];
                    for (var d in data) {
                        if (data.hasOwnProperty(d))
                            str.push(encodeURIComponent(d) + '=' + encodeURIComponent(data[d]));
                    }
                    return str.join('&');
                }
            }
        });

        function _register(user) {
            var _user = new Register(user);
            return _user.$save()
            .catch(function (err) {
                console.error(err.data);
                return $q.reject(err.data);
            });
        };

        function _login(user) {
            user.grant_type = 'password';
            var _user = new Login(user);
            return _user.$save()
                .then(function (data, headers) {
                    debugger;
                    $localStorage.authData = {
                        userName: data.userName,
                        token: data.access_token,
                        _expiration: new Date(data['.expires'])
                    };

                    var returnUrl = $routeParams.returnUrl ? decodeURIComponent($routeParams.returnUrl) : '/';
                    $location.path(returnUrl).search('returnUrl', null);
                    return $q.resolve({
                        isAuth: $localStorage.authData && true,
                        userName: $localStorage.authData.userName,
                    });
                })
            .catch(function (err) {
                console.error(err.data.error_description);
                delete $localStorage.authData;
                return $q.reject(err.data);
            });
        };

        function _logout() {
            delete $localStorage.authData;
        }

        return {
            register: _register,
            login: _login,
            logout: _logout,
            getAuthentificationInfo: function () {
                if ($localStorage.authData && (new Date($localStorage.authData._expiration) < Date.now()))
                    delete $localStorage.authData;

                return {
                    isAuth: $localStorage.authData && true,
                    userName: $localStorage.authData ? $localStorage.authData.userName : null
                };
            }
        }
    }]);

    OutOfRangeApp.factory('authInterceptorService', ['$q', '$location', '$localStorage', function ($q, $location, $localStorage) {

        function _request(config) {
            config.headers = config.headers || {};

            if (config.url === '/token')
                return config;

            if ($localStorage.authData && (new Date($localStorage.authData._expiration) < Date.now()))
                delete $localStorage.authData;

            var authData = $localStorage.authData;
            if (authData) {
                config.headers.Authorization = 'Bearer ' + authData.token;
            }
            return config;
        };

        function _responseError(rejection) {
            if (rejection.status === 401) {
                var oldUrl = $location.path()
                $location.path('/login').search('returnUrl', encodeURIComponent(oldUrl))
            }
            return $q.reject(rejection);
        }

        return {
            request: _request,
            responseError: _responseError
        }
    }]);

    OutOfRangeApp.config(['$httpProvider', function ($httpProvider) {
        $httpProvider.interceptors.push('authInterceptorService');
    }]);

    OutOfRangeApp.factory('qaService', ['$resource', '$location', function ($resource, $location) {
        var Questions = $resource('/api/questions');
        return {
            getQuestions: function () {
                return new Questions().$query()
            }
        }
    }]);

})();