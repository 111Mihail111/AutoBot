setInterval(function () {
    debugger;
    let notes = document.getElementById("Notes");
    for (var i = 0; i < notes.childElementCount; i++) {
        let timer = notes.children[i].getElementsByTagName("input")[1].value;
        if (timer === "00:00:00") {

            let statusCrane = notes.children[i].getElementsByTagName("input")[2].value;
            if (statusCrane === "NoWork") {
                continue;
            }

            GoToCrane(GetDataCrane(notes.children[i]));
        }
        else {
            UpdatingTimerCrane(GetDataCrane(notes.children[i]));
        }
    }
}, 420000)


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
            debugger;

            //TODO:Отладить метод
            CheckTimers(crane);
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


function CheckTimers(crane) {
    var row = document.getElementById("Notes");
    var list = [];

    for (var i = 0; i < row.childElementCount; i++) {
        list[i] = GetDataCrane(row.children[i]);
    }

    for (var i = 0; i < list.length; i++) {
        var element = list[i];
        if (crane.URL === element.URL && element.ActivityTime === "00:00:00") {
            GoToCrane(element);
        }
    }
}