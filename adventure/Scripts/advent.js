/**
 * Created by Adrian on 03/10/2015.
 */
$(function () {

    //"Days": [
    //    {
    //        "Day": 1,
    //        "Challenges": [
    //            {
    //                "Challenge": {
    //                    "Title": "Challenge for December 1",
    //                    "Hashtag": "#AdventureDay1"
    //                }
    //            },
    //            {
    //                "Challenge": {
    //                    "Title": "Bonus challenge for December 1",
    //                    "Hashtag": "#AdventureDay1_Bonus1"
    //                }
    //            }
    //        ]
    //    }

    //var url = "http://adventure-1.apphb.com/api/days";
    var url = "data.json";

    getDataFromServer();

    function processData(data) {
        console.log(data);
        var str;
        for (i=0; i < data.Days.length; i++) {
            str = "Day: " + data.Days[i].Day;
            str += " - Number of challenges: " + data.Days[i].Challenges.length;
            str += "<ul>";
            for (j=0; j < data.Days[i].Challenges.length; j++) {
                str += "<br/>";
                str += "<li>" + data.Days[i].Challenges[j].Challenge.Title + " " + data.Days[i].Challenges[j].Challenge.Hashtag + "</li>"
            }
            str += "</ul><br/>";
            $("body").append(str);
        }
    }

    function getDataFromServer() {
        var xhr = $.getJSON( url, function() {
        })
            .done(function(data) {
                processData(data);
            })
            .fail(function( jqxhr, textStatus, error ) {
                var err = textStatus + ", " + error;
                console.log("Request Failed: " + err);
            });
    }
});