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
                    var allDates = [],
                        today = parseInt( new Date().getDate() );

                    Adventure.Ajax.Retrieve( ajax_url, $q ).then( function( dates ) {
                        if ( dates !== undefined ) {
                            for (var i = 0; i < dates.length; i++) {
                                if ( today >= dates[ i ].Challenge.ChallengeNumber ) {
                                    dates[ i ].available = true;
                                }
                                allDates.push( dates[ i ] );
                            }
                            $scope.days = allDates;
                        }
                    });

                },

                Day: function ( $scope, $q, $state ) {

                    this.params = $state.params;
                    var day_id = this.params.day_id,
                        day;

                    $.get( 'public/content/details/' + day_id + '.html' ).then(function( response ) {
                        $( '.slab--content' ).html( response );
                    });

                    Adventure.Ajax.Retrieve( ajax_url, $q ).then( function( dayslist ) {
                        if ( dayslist !== undefined ) {
                            dayslist = Adventure.ProcessDates( dayslist );

                            for (var i = 0; i < dayslist.length; i++) {
                                if ( dayslist[ i ].Day == day_id ) {
                                    $scope.day = dayslist[ i ];
                                    $scope.tweet = Adventure.Twitter.GetLink( $scope.day );
                                }
                            }
                        }
                    });

                },

                DayEntries: function ( $scope, $q, $state ) {

                    this.params = $state.params;
                    var day_id = this.params.day_id,
                        day;

                    Adventure.Ajax.Retrieve( ajax_url, $q ).then( function( dayslist ) {
                        if ( dayslist !== undefined ) {
                            dayslist = Adventure.ProcessDates( dayslist );

                            for (var i = 0; i < dayslist.length; i++) {
                                if ( dayslist[ i ].Day == day_id ) {
                                    $scope.day = dayslist[ i ];
                                    $scope.tweet = Adventure.Twitter.GetLink( $scope.day );
                                }
                            }
                        }
                    });

                },

                DayDetails: function ( $scope, $q, $state ) {

                    this.params = $state.params;
                    var day_id = this.params.content_id,
                        day;

                    $.get( 'public/content/more/' + day_id + '.html' ).then(function( response ) {
                        $( '.slab--content' ).html( response );
                    });

                    Adventure.Ajax.Retrieve( ajax_url, $q ).then( function( dayslist ) {
                        if ( dayslist !== undefined ) {
                            dayslist = Adventure.ProcessDates( dayslist );

                            for (var i = 0; i < dayslist.length; i++) {
                                if ( dayslist[ i ].Day == day_id ) {
                                    $scope.day = dayslist[ i ];
                                    $scope.tweet = Adventure.Twitter.GetLink( $scope.day );
                                }
                            }
                        }
                    });

                },

                Days: function ( $scope, $q ) {

                    $scope.days = [];
                    Adventure.Ajax.Retrieve( ajax_url, $q ).then( function( dayslist ) {
                        if ( dayslist !== undefined ) {
                            dayslist = Adventure.ProcessDates( dayslist );
                            $scope.days = dayslist;
                        }
                    });

                },

                User: function ( $scope, $q, $state ) {

                    var user_id = $state.params.user_id;
                    $scope.user = [];
                    Adventure.Ajax.Retrieve( 'http://adventure-1.apphb.com/api/user/' + user_id, $q ).then( function( user ) {
                        if ( user !== undefined ) {
                            $scope.user = user;
                        }
                    });

                },

                UserProfile: function ( $scope, $q, $state, twitterService ) {

                    var user_id = '';
                    $scope.user = null;

                    var service = twitterService.isReady();

                    if ( service ) {
                        service.me().done(function (result) {
                            user_id = result.id;

                            Adventure.Ajax.Retrieve( 'http://adventure-1.apphb.com/api/user/' + user_id, $q ).then( function( user ) {
                                if ( user !== undefined ) {
                                    $scope.user = user;
                                }
                            });
                        });
                    }

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
                    };

                    $scope.rankingVisible = function (position) {
                      var topTen = position < 10;
                      var nearMe = Math.abs(position - $scope.myPosition) < 4;
                      return topTen || nearMe;
                    };
                },

                Badges: function () {

                    

                },

                FourOhFour: function () {

                    

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
                        url: "/day/{content_id}/more",
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
                    .state('user/profile', {
                        url: "/user/profile",
                        templateUrl: 'Public/templates/user-profile.html',
                        controller: Adventure.Angular.Controller.UserProfile,
                        controllerAs: 'user'
                    })
                    .state('user', {
                        url: "/user/{user_id}",
                        templateUrl: 'Public/templates/user.html',
                        controller: Adventure.Angular.Controller.User,
                        controllerAs: 'user'
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

            var pastDates = [],
                today = parseInt( new Date().getDate() );

            if ( dates !== undefined ) {
                for (var i = 0; i < dates.length; i++) {
                    if ( today >= dates[ i ].Challenge.ChallengeNumber ) {
                        pastDates.push( dates[ i ] );
                    }
                }
            }
            return pastDates;
        },

        Twitter: {

            Controller: function ( $scope, $q, twitterService, $state ) {

                $scope.tweets = [];
                
                twitterService.initialize();
                
                $scope.Connect = function() {
                    twitterService.connectTwitter().then(function() {
                        if (twitterService.isReady()) {
                            $state.reload();
                        }
                    });
                };

                $scope.SignOut = function() {
                    twitterService.clearCache();
                    $scope.tweets.length = 0;
                    $('#getTimelineButton, #signOut').fadeOut(function(){
                        $('#connectButton').fadeIn();
                    });
                };

                if (twitterService.isReady()) {

                }

            },

            GetLink: function ( day ) {

                return '<a href="https://twitter.com/intent/tweet?text=%23AdventHunt' + day.Day + '%20%23submit&original_referer=' + window.location.href + '" class="button tweet">Tweet your entry</a>';

            }

        }

    };
}());

var app = angular.module( 'adventure', [ 'ui.router', 'twitterApp.services', 'ngSanitize' ] );

app.controller('TwitterController', Adventure.Twitter.Controller );

app.config(['$stateProvider', '$urlRouterProvider', Adventure.Angular.Router ]);

$( Adventure.Init );
