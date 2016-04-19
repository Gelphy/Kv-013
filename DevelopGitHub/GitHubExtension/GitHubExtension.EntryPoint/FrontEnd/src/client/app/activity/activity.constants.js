(function () {
    'use strict';

    angular.module('app.activity')
        .constant('ACTIVITY_CONSTANTS', (function () {
                return {
                    'ACTIVITY_PATH_PREFIX': 'Frontend/src/client/app/activity/',
                    'ACTIVITY_PATH_POSTFIX': '-activity.html',
                    'EXTERNAL_ACTIVITY_ITEMS_PER_PAGE': 30,
                    'FIRST_PAGE': 1
                };
            })()
        );
})();
