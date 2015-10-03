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

                Main: function ( $scope, $q ) {

                    $scope.days = [];
                    var challenges = [],
                        challenge;

                    Adventure.Ajax.Retrieve( ajax_url, $q ).then( function( dayslist ) {
                        if ( dayslist !== undefined ) {
                            dayslist = dayslist.Days;
                            for (var i = 0; i < dayslist.length; i++) {
                                for (var j = 0; j < dayslist[ i ].Challenges.length; j++) {
                                    challenge = dayslist[ i ].Challenges[ j ];
                                    challenge.Day = dayslist[ i ].Day;
                                    challenge.challenge_id = j;
                                    challenge.ilk = challenge.Challenge.Hashtag.toLowerCase().indexOf("bonus") >= 0 ? 'bonus' : 'standard';
                                    challenges.push( challenge );
                                }
                            }
                            $scope.days = challenges;
                        }
                    });

                },

                Day: function ( $scope, $q, $state ) {

                    this.params = $state.params;
                    var day_id = this.params.day_id,
                        challenge_id = this.params.challenge_id !== undefined && this.params.challenge_id ? parseInt( this.params.challenge_id ) : 0,
                        challenge,
                        day;

                    Adventure.Ajax.Retrieve( ajax_url, $q ).then( function( dayslist ) {
                        if ( dayslist !== undefined ) {
                            dayslist = Adventure.ProcessDates( dayslist );

                            for (var i = 0; i < dayslist.length; i++) {
                                if ( dayslist[ i ].Day == day_id ) {
                                    day = dayslist[ i ];

                                    if ( day.Challenges[ challenge_id ] !== undefined ) {
                                        challenge = day.Challenges[ challenge_id ];
                                        challenge.Day = day_id;
                                        $scope.day = challenge;
                                    }
                                }
                            }
                        }
                    });

                },

                DayEntries: function ( $scope, $q, $state ) {

                    this.params = $state.params;
                    var day_id = this.params.day_id,
                        challenge_id = this.params.challenge_id !== undefined && this.params.challenge_id ? parseInt( this.params.challenge_id ) : 0,
                        challenge,
                        day;

                    Adventure.Ajax.Retrieve( ajax_url, $q ).then( function( dayslist ) {
                        if ( dayslist !== undefined ) {
                            dayslist = Adventure.ProcessDates( dayslist );

                            for (var i = 0; i < dayslist.length; i++) {
                                if ( dayslist[ i ].Day == day_id ) {
                                    day = dayslist[ i ];

                                    if ( day.Challenges[ challenge_id ] !== undefined ) {
                                        challenge = day.Challenges[ challenge_id ];
                                        challenge.Day = day_id;
                                        $scope.day = challenge;
                                    }
                                }
                            }
                        }
                    });

                },

                DayDetails: function ( $scope, $q, $state ) {

                    this.params = $state.params;
                    var content_id = this.params.content_id,
                        challenge,
                        day;

                    $scope.the_content = '';

                    $.get( 'public/content/' + content_id + '.html' ).then(function( response ) {
                        $( '.slab--content' ).html( response );
                    });

                    Adventure.Ajax.Retrieve( ajax_url, $q ).then( function( dayslist ) {
                        if ( dayslist !== undefined ) {
                            dayslist = Adventure.ProcessDates( dayslist );

                            for (var i = 0; i < dayslist.length; i++) {
                                if ( dayslist[ i ].Day == content_id ) {

                                    day = dayslist[ i ];
                                    if ( day.Challenges[ 0 ] !== undefined ) {
                                        day.Challenge = day.Challenges[ 0 ];
                                    }

                                    $scope.day = day;
                                }
                            }
                        }
                    });

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

                Rankings: function ( $scope, $urlRouter ) {

                    this.params = $urlRouter;

                    $scope.myPosition = 25;
                    $scope.rankings = [
                      {"UserName":"Gus","Points":120},
                      {"UserName":"John","Points":99},
                      {"UserName": "Fireman Sam Long Name","Points":1},
                      {"UserName":"Gus","Points":120},
                      {"UserName":"John","Points":99},
                      {"UserName": "Fireman Sam Long Name","Points":1},
                      {"UserName":"Gus","Points":120},
                      {"UserName":"John","Points":99},
                      {"UserName": "Fireman Sam Long Name","Points":1},
                      {"UserName":"Gus","Points":120},
                      {"UserName":"John","Points":99},
                      {"UserName": "Fireman Sam Long Name","Points":1},
                      {"UserName":"Gus","Points":120},
                      {"UserName":"John","Points":99},
                      {"UserName": "Fireman Sam Long Name","Points":1},
                      {"UserName":"Gus","Points":120},
                      {"UserName":"John","Points":99},
                      {"UserName": "Fireman Sam Long Name","Points":1},
                      {"UserName":"Gus","Points":120},
                      {"UserName":"John","Points":99},
                      {"UserName": "Fireman Sam Long Name","Points":1},
                      {"UserName":"Gus","Points":120},
                      {"UserName":"John","Points":99},
                      {"UserName": "Fireman Sam Long Name","Points":1},
                      {"UserName":"Gus","Points":120},
                      {"UserName":"John","Points":99},
                      {"UserName": "Fireman Sam Long Name","Points":1},
                    ];

                    $scope.positionClass = function (position) {
                      var classes = "slab ranking";
                      if (position === $scope.myPosition) {
                        classes += " ranking__me";
                      }

                      return classes;
                    }

                    $scope.rankingVisible = function (position) {
                      var topTen = position < 10;
                      var nearMe = Math.abs(position - $scope.myPosition) < 4;
                      return topTen || nearMe;
                    }
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
                        templateUrl: 'Public/templates/days.html',
                        controller: Adventure.Angular.Controller.Days
                    })
                    .state('day/:content_id', {
                        url: "/day/{content_id}/details",
                        templateUrl: 'public/templates/day-details.html',
                        controller: Adventure.Angular.Controller.DayDetails,
                        controllerAs: 'day'
                    })
                    .state('day', {
                        url: "/day/{day_id}",
                        templateUrl: 'Public/templates/day.html',
                        controller: Adventure.Angular.Controller.Day,
                        controllerAs: 'day'
                    })
                    .state('day/:day_id', {
                        url: "/day/{day_id}/entries",
                        templateUrl: 'public/templates/day-entries.html',
                        controller: Adventure.Angular.Controller.DayEntries,
                        controllerAs: 'day'
                    })
                    .state('rankings', {
                        url: "/rankings",
                        templateUrl: 'Public/templates/rankings.html',
                        controller: Adventure.Angular.Controller.Rankings
                    })
                    .state('badges', {
                        url: "/badges",
                        templateUrl: 'Public/templates/badges.html',
                        controller: Adventure.Angular.Controller.Badges
                    })
                    .state('index', {
                        url: '/',
                        templateUrl: 'Public/templates/main.html',
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
                console.log("Day: " + day_id + " Num Challenges: " + num_chals + " " + chal_title);
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
