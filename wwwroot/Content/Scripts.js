setInterval(function () {
    //CheckSites("Cranes", true);
    CheckSites("InternetService", false);
}, 60000)

/**
 * Проверка сайтов
 * @param {any} site Id контейнера
 * @param {any} isCrane Является ли краном
 */
function CheckSites(site, isCrane) {
    let notes = document.getElementById(site);
    for (var i = 2; i < notes.childElementCount; i++) {
        let timer = notes.children[i].getElementsByTagName("input")[1].value;
        if (timer === "00:00:00") {

            let statusCrane = notes.children[i].getElementsByTagName("input")[2].value;
            if (statusCrane === "NoWork" || statusCrane === "InWork") {
                continue;
            }

            notes.children[i].getElementsByTagName("input")[2].value = "InWork";

            var data;
            if (isCrane) {
                data = GetDataCrane(notes.children[i]);
                UpdatingStatusCrane(data.URL, data.StatusCrane);
                GoToCrane(data);
            }
            else {
                data = GetDataService(notes.children[i]);
                UpdatingStatusService(data.URL, data.StatusService);
                GoToService(data);
            }
        }
        else {
            if (isCrane) {
                UpdatingTimerCrane(GetDataCrane(notes.children[i]));
                return;
            }
            UpdatingTimerService(GetDataService(notes.children[i]));
        }
    }
}

/**
 * Смена статус
 * @param {any} select DropDownlList-контрол
 */
function ChangeStatus(select) {
    var row = select.parentElement.parentElement.parentElement;
    row.children[0].children[2].children[0].value = select.value;
}

/*Получить данные крана*/
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
/*Получить данные сервиса*/
function GetDataService(row) {
    return {
        URL: row.getElementsByTagName("input")[0].value,
        ActivityTime: row.getElementsByTagName("input")[1].value,
        StatusService: row.getElementsByTagName("input")[2].value,
        BalanceOnService: row.getElementsByTagName("input")[3].value,
        TypeService: row.getElementsByTagName("input")[4].value,
    }
}

/*Обновить таймер крана*/
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
            $('#Cranes').html(data);
            CheckTimersCrane(crane);
        }
    });
}
/*Обновить таймер сервиса*/
function UpdatingTimerService(internetService) {
    $.ajax({
        type: "GET",
        data: {
            'URL': internetService.URL,
            'ActivityTime': internetService.ActivityTime,
            'StatusService': internetService.StatusService,
            'BalanceOnService': internetService.BalanceOnService,
            'TypeService': internetService.TypeService,
        },
        contentType: "application/json; charset=utf-8",
        url: "/Start/UpdateTimerService",
        success: function (data) {
            $('#InternetService').html(data);
            CheckTimersInternetService(internetService);
        }
    });
}

/*Обновить статус крана*/
function UpdatingStatusCrane(url, status) {
    $.ajax({
        type: "GET",
        data: {
            url: url,
            statusCrane: status,
        },
        contentType: "application/json; charset=utf-8",
        url: "/Start/UpdateStatusCrane/",
        success: function (data) {
            $('#Cranes').html(data);
        }
    });
}
/*Обновить статус сервиса*/
function UpdatingStatusService(url, status) {
    $.ajax({
        type: "GET",
        data: {
            url: url,
            statusService: status,
        },
        contentType: "application/json; charset=utf-8",
        url: "/Start/UpdateStatusService/",
        success: function (data) {
            $('#InternetService').html(data);
        }
    });
}

/*Перейти на кран*/
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
            $('#Cranes').html(data);
        }
    });
}
/*Перейти на сервис*/
function GoToService(internetService) {
    $.ajax({
        type: "GET",
        data: {
            'URL': internetService.URL,
            'ActivityTime': internetService.ActivityTime,
            'StatusService': internetService.StatusService,
            'BalanceOnService': internetService.BalanceOnService,
            'TypeService': internetService.TypeService,
        },
        url: "/Start/GoToInternetService",
        success: function (data) {
            $('#InternetService').html(data);
        }
    });
}

/*Проверка таймеров*/
function CheckTimersCrane(crane) {
    var row = document.getElementById("Cranes");
    var list = [];

    for (var i = 2; i < row.childElementCount; i++) {
        list[i - 2] = GetDataCrane(row.children[i]);
    }

    for (var i = 0; i < list.length; i++) {
        var element = list[i];
        if (crane.URL === element.URL && element.ActivityTime === "00:00:00") {
            CheckSites("Cranes", true);
        }
    }
}

/*Проверка таймеров*/
function CheckTimersInternetService(internetService) {
    var row = document.getElementById("InternetService");
    var list = [];

    for (var i = 2; i < row.childElementCount; i++) {
        list[i - 2] = GetDataService(row.children[i]);
    }

    for (var i = 0; i < list.length; i++) {
        var element = list[i];
        if (internetService.URL === element.URL && element.ActivityTime === "00:00:00") {
            CheckSites("InternetService", false);
        }
    }
}