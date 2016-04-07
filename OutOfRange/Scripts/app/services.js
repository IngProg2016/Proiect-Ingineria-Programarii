(function () {
    'use strict';

    var OutOfRangeApp = angular.module('OutOfRangeApp');

    OutOfRangeApp.factory('authService', ['$q', '$resource', '$location', '$routeParams', 'storageService', function ($q, $resource, $location, $routeParams, storageService) {
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
                .then(function (data) {
                    $location.path('/login');
                    return $q.resolve(data);
                })
                .catch(function (err) {
                    console.error(err.data);
                    return $q.reject(err.data);
                });
        };

        function _login(user, rememberMe) {
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

                    var returnUrl = $routeParams.returnUrl ? decodeURIComponent($routeParams.returnUrl) : '/';
                    $location.path(returnUrl).search('returnUrl', null);
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

        function _logout() {
            storageService.auth.remove();
        }



        return {
            register: _register,
            login: _login,
            logout: _logout,
            getAuthentificationInfo: function () {
                return storageService.auth.get();
            }
        }
    }]);

    OutOfRangeApp.factory('storageService', ['$localStorage', '$sessionStorage', function ($localStorage, $sessionStorage) {
        function _getAuthData() {
            var authData = $sessionStorage.authData || $localStorage.authData;

            if (authData && (new Date(authData._expiration) < Date.now()))
                _deleteAuthData();

            if (authData) {
                authData._expiration = new Date(authData._expiration);
                return authData;
            }
            return { isAuth: false };
        }

        function _setAuthData(rememberMe, data) {
            if (rememberMe)
                $localStorage.authData = data;
            else
                $sessionStorage.authData = data;
        }

        function _deleteAuthData() {
            delete $localStorage.authData;
            delete $sessionStorage.authData;
        }

        return {
            auth: {
                get: _getAuthData,
                set: _setAuthData,
                remove: _deleteAuthData
            }
        }
    }]);

    OutOfRangeApp.factory('authInterceptorService', ['$q', '$location', 'storageService', function ($q, $location, storageService) {

        function _request(config) {
            config.headers = config.headers || {};

            if (config.url === '/token')
                return config;

            var authData = storageService.auth.get();
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
                return Questions.query().$promise;
            }
        }
    }]);

})();