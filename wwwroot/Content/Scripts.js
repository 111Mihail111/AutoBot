setInterval(function () {
    let notes = document.getElementById("Notes");
    for (var i = 0; i < notes.childElementCount; i++) {
        let timer = notes.children[i].getElementsByTagName("input")[1].value;
        if (timer === "00:00:00") {

            let statusCrane = notes.children[i].getElementsByTagName("input")[2].value;
            if (statusCrane === "NoWork") {
                continue;
            }
            debugger;
            GoToCrane(GetDataCrane(notes.children[i]));
        }
        else {
            UpdatingTimerCrane(GetDataCrane(notes.children[i]));
        }
    }
}, 10000)


function GetDataCrane(row) {
    return {
        URL: row.getElementsByTagName("input")[0].value,
        ActivityTime: row.getElementsByTagName("input")[1].value,
        StatusCrane: row.getElementsByTagName("input")[2].value,
        BalanceOnCrane: row.getElementsByTagName("input")[3].value,
        TypeCurrencies: row.getElementsByTagName("input")[4].value,
        TypeCrane: row.getElementsByTagName("input")[5].value,
    }
}


function UpdatingTimerCrane(crane) {
    $.ajax({
        type: "GET",
        data: {
            'URL': crane.URL,
            'ActivityTime': crane.ActivityTime,
            'StatusCrane': crane.StatusCrane,
            'BalanceOnCrane': crane.BalanceOnCrane,
            'TypeCurrencies': crane.TypeCurrencies,
            'TypeCrane': crane.TypeCrane,
        },
        contentType: "application/json; charset=utf-8",
        url: "/Start/UpdateTimerCrane",
        success: function (data) {
            $('#Notes').html(data);

            if (data.ActivityTime === "00:00:00") {
                GoToCrane(data);
            }
        }
    });
}


function GoToCrane(crane) {
    $.ajax({
        type: "GET",
        data: {
            'URL': crane.URL,
            'ActivityTime': crane.ActivityTime,
            'StatusCrane': crane.StatusCrane,
            'BalanceOnCrane': crane.BalanceOnCrane,
            'TypeCurrencies': crane.TypeCurrencies,
            'TypeCrane': crane.TypeCrane,
        },
        url: "/Start/GoToCrane",
        success: function (data) {
            $('#Notes').html(data);
        }
    });
}