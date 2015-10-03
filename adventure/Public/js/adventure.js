var Adventure = (function () {
    "use strict";

    var ajax_url = '';

    return {
        Init: function () {
            
            ajax_url = "Test/data.json";

            if (location.href.indexOf("apphb") >= 0)
                ajax_url = "http://adventure-1.apphb.com/api/days";

        },

        Angular: {

            Controller: {

                Main: function ( $scope ) {

                    // console.log( $scope );

                },

                Day: function ( $urlRouter ) {

                    console.log( 'b' );
                    this.params = $urlRouter;

                },

                Days: function ( $scope, $urlRouter, $q ) {

                    $scope.params = $urlRouter;
                    $scope.days = [];
                    Adventure.Ajax.Retrieve( ajax_url, $q ).then( function( dayslist ) {
                        if ( dayslist !== undefined ) {
                            dayslist = Adventure.ProcessDates( dayslist );
                            $scope.days = dayslist;
                        }
                    });

                },

                Rankings: function ( $urlRouter ) {

                    this.params = $urlRouter;

                },

                Badges: function ( $urlRouter ) {

                    this.params = $urlRouter;

                },

                FourOhFour: function ( $urlRouter ) {

                    this.params = $urlRouter;

                }

            },

            Router: function ( $stateProvider, $urlRouterProvider ) {

                // $urlRouterProvider.otherwise( '/404' );

                $stateProvider
                    .state('days', {
                        url: "/days",
                        templateUrl: 'public/templates/days.html',
                        controller: Adventure.Angular.Controller.Days
                    })
                    .state('day', {
                        url: "/day/{day_id}",
                        templateUrl: 'public/templates/day.html',
                        controller: Adventure.Angular.Controller.Day,
                        controllerAs: 'day'
                    })
                    .state('rankings', {
                        url: "/rankings",
                        templateUrl: 'public/templates/rankings.html',
                        controller: Adventure.Angular.Controller.Rankings
                    })
                    .state('badges', {
                        url: "/badges",
                        templateUrl: 'public/templates/badges.html',
                        controller: Adventure.Angular.Controller.Badges
                    })
                    .state('index', {
                        url: '/',
                        templateUrl: 'public/templates/main.html',
                        controller: Adventure.Angular.Controller.Main,
                    });

            }

        },

        Ajax: {

            Get: function ( url, callback ) {

                var xhr = $.getJSON( url, callback )
                    .fail(function( jqxhr, textStatus, error ) {
                        var err = textStatus + ", " + error;
                        console.log("Request Failed: " + err);
                    });

            },

            Retrieve: function ( url, $q ) {

                var deferred = $q.defer();
                var promise = $.getJSON( url ).done(function(data) {
                    deferred.resolve(data);
                });

                return deferred.promise;
            },

            Process: function ( response ) {
                
                var dayno = 0,
                    day_id = response.Days[dayno].Day,
                    num_chals = response.Days[dayno].Challenges.length,
                    chal = response.Days[dayno].Challenges,
                    chal_title = response.Days[dayno].Challenges[0].Challenge.Title;

                console.log("JSON loaded....");
                console.log("Day: "+day_id+ " Num Challenges: "+num_chals+ " "+chal_title);
            }

        },

        ProcessDates: function ( dates ) {
            
            var days = dates.Days,
                pastDates = [],
                today = parseInt( new Date().getDate() );

            if ( days !== undefined ) {
                for (var i = 0; i < days.length; i++) {
                    if ( today >= days[ i ].Day ) {
                        pastDates.push( days[ i ] );
                    }
                }
            }
            return pastDates;
        }

    };
}());

var app = angular.module( 'adventure', [ 'ui.router' ] );

app.config(['$stateProvider', '$urlRouterProvider', Adventure.Angular.Router ]);

$( Adventure.Init );