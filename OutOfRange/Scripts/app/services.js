(function () {
    'use strict';

    var OutOfRangeApp = angular.module('OutOfRangeApp');

    OutOfRangeApp.factory('Auth', ['$resource', '$localStorage', function ($resource, $localStorage) {
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

        var _authentication = {
            isAuth: false,
            userName: '',
            token: '',
            _expiration: 0
        };

        function _register(user) {
            var _user = new Register(user);
            _user.$save();
        };

        function _login(user) {
            user.grant_type = 'password';
            var _user = new Login(user);
            _user.$save().then(function (data, headers) {
                debugger;
                $localStorage.authData = {
                    userName: data.userName,
                    token: data.access_token,
                    _expiration: new Date(data['.expires'])
                }
                _fillAuthData();
                return _authentication;
            });
        };

        function _logout() {
            _authentication = { isAuth: false };
        }

        function _fillAuthData() {
            var authData = $localStorage.authData;
            debugger;
            if (authData) {
                _authentication.isAuth = true;
                _authentication.userName = authData.userName;
                _authentication.token = authData.token;
                _authentication._expiration = authData._expiration;
            }
        };

        return {
            register: _register,
            login: _login,
            logout: _logout,
            getAuthenticationInfo: function () {
                return {
                    isAuth: _authentication.isAuth,
                    userName: _authentication.userName,
                };
            },
            fillAuthData: _fillAuthData
        }
    }]);


    OutOfRangeApp.run(['Auth', function (Auth) {
        Auth.fillAuthData();
    }]);

})();