var Adventure = (function () {
    "use strict";

    var ajax_url = '';

    return {
        Init: function () {

            ajax_url = "Test/data.json";

            if (location.href.indexOf("apphb") >= 0)
                ajax_url = "http://adventure-1.apphb.com/api/days";

            if ( ! $( '.badge-link' ).hasClass( 'has-user' ) ) {
                // console.log( twitterService.isReady() );
            }

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

                    Adventure.Ajax.Retrieve('http://adventure-1.apphb.com/api/challenge/' + day_id, $q).then(function (responses) {
                        if (responses !== undefined) {
                            var responseArray = [];
                            $scope.responses = [];
                            for (var i = 0; i < responses.length; i++) {
                                responseArray.push({
                                    user: responses[i].User,
                                    tweet: responses[i].Tweet,
                                    tweetId: responses[i].TweetId
                                });
                            }
                            $scope.responses = responseArray;
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

                UserProfile: function ( $scope, $q, twitterService ) {

                    var user_id = Adventure.GetCookie( 'adventureTwitter' );
                    $scope.user = null;

                    if ( user_id ) {
                        Adventure.Ajax.Retrieve( 'http://adventure-1.apphb.com/api/user/' + user_id, $q ).then( function( user ) {
                            if ( user !== undefined ) {
                                $scope.user = user;
                            }
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

                Badges: function ( $scope, $q, twitterService ) {

                    var user_id = Adventure.GetCookie( 'adventureTwitter' );
                    $scope.user = null;

                    if ( user_id ) {                  
                        Adventure.Ajax.Retrieve( 'http://adventure-1.apphb.com/api/user/' + user_id, $q ).then( function( user ) {
                            if ( user !== undefined ) {
                                var badges = [];
                                if ( user.UserBadges !== undefined ) {
                                    for (var i = 0; i < user.UserBadges.length; i++) {
                                        badges.push( user.UserBadges[ i ].BadgeId );
                                    }
                                }
                                $scope.badges = badges;
                                $scope.user = user;
                            }
                        });
                    }

                },

                FourOhFour: function () {

                    

                }

            },

            Router: function ( $stateProvider, $urlRouterProvider ) {

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
                    .state('badges.id', {
                        url: "/badges/:user_id",
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
                
                twitterService.initialize();
                
                $scope.Connect = function() {
                    var cookie = Adventure.GetCookie( 'adventureTwitter' );
                    if ( cookie ) {
                        return cookie;
                    } else {
                        OAuth.popup('twitter', {cache:true} )
                            .done( function ( result ) {
                                result.me()
                                    .done( function ( response ) {
                                        document.cookie = 'adventureTwitter=Har 10t;expires=Fri, 25 Dec 2015 23:59:59 GMT';
                                        $state.go($state.current, {}, {reload: true});
                                    });
                            });
                    }
                };

                $scope.SignOut = function() {
                    twitterService.clearCache();
                };

                if (twitterService.isReady()) {

                }

            },

            GetLink: function ( day ) {

                return '<a href="https://twitter.com/intent/tweet?text=%23AdventHunt' + day.Day + '%20%23submit&original_referer=' + window.location.href + '" class="button tweet">Tweet your entry</a>';

            }

        },

        GetCookie: function ( key ) {
            return decodeURIComponent(document.cookie.replace(new RegExp("(?:(?:^|.*;)\\s*" + encodeURIComponent(key).replace(/[\-\.\+\*]/g, "\\$&") + "\\s*\\=\\s*([^;]*).*$)|^.*$"), "$1")) || null;
        },

        KillCookie: function ( key ) {
            var sDomain = '',
                sPath = '';
            return document.cookie = encodeURIComponent(key) + "=; expires=Thu, 01 Jan 1970 00:00:00 GMT" + (sDomain ? "; domain=" + sDomain : "") + (sPath ? "; path=" + sPath : "");
        }

    };
}());

var app = angular.module( 'adventure', [ 'ui.router', 'twitterApp.services', 'ngSanitize' ] );

app.controller('TwitterController', Adventure.Twitter.Controller );

app.config(['$stateProvider', '$urlRouterProvider', Adventure.Angular.Router ]);

$( Adventure.Init );
