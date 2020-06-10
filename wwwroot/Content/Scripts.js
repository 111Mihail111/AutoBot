﻿setInterval(function () {
    let notes = document.getElementById("Notes");
    for (var i = 0; i < notes.childElementCount; i++) {
        let timerRow = notes.children[i].children[2].innerText;
        if (timerRow === "00:00:00") {
            //Ajax запрос с переходом на сайт
            debugger;
            GoToCrane(GetDataCrane(notes.children[i]));
        }
        else {
            //Ajax запрос с обновление таймера
            UpdatingTimerCrane(GetDataCrane(notes.children[i]));
        }
    }
}, 10000)


function GetDataCrane(row) {
    return {
        URL: row.children[1].innerText,
        ActivityTime: row.children[2].innerText,
        StatusCrane: row.children[3].innerText,
        MyBalanceOnCrane: row.children[4].innerText,
        TypeCurrencies: row.children[5].innerText,
    }
}

function UpdatingTimerCrane(crane) {
    $.ajax({
        type: "GET",
        data: {
            'URL': crane.URL,
            'ActivityTime': crane.ActivityTime,
            'StatusCrane': crane.StatusCrane,
            'MyBalanceOnCrane': crane.MyBalanceOnCrane,
            'TypeCurrencies': crane.TypeCurrencies,
        },
        contentType: "application/json; charset=utf-8",
        url: "/Start/UpdateTimerCrane",
        success: function (data) {
            $('#Notes').html(data);
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
            'MyBalanceOnCrane': crane.MyBalanceOnCrane,
            'TypeCurrencies': crane.TypeCurrencies,
        },
        url: "/Start/GoToCrane",
    });
}