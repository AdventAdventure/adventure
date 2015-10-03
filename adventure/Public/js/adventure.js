
var url = "Test/data.json";

if (location.href.indexOf("apphb") >= 0)
    url = "http://adventure-1.apphb.com/api/days";

getDataFromServer();

function processData(data) {
    var dayno = 0;

    var day_id = data.Days[dayno].Day;
    var num_chals = data.Days[dayno].Challenges.length;
    var chal = data.Days[dayno].Challenges;
    var chal_title = data.Days[dayno].Challenges[0].Challenge.Title;

    console.log("JSON loaded....");
    console.log("Day: "+day_id+ " Num Challenges: "+num_chals+ " "+chal_title);
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