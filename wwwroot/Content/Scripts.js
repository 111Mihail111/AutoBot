﻿setInterval(function () {
    CheckSites("Cranes", true);
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

            let status = notes.children[i].getElementsByTagName("input")[2].value;
            if (status === "NoWork" || status === "InWork") {
                continue;
            }

            notes.children[i].getElementsByTagName("input")[2].value = "InWork";

            var data;
            if (isCrane) {
                data = GetDataRow(notes.children[i]);
                UpdatingStatusCrane(data.URL, data.Status);
                GoToCrane(data);
            }
            else {
                data = GetDataRow(notes.children[i]);
                UpdatingStatusService(data.URL, data.Status);
                GoToService(data);
            }
        }
        else {
            if (isCrane) {
                UpdatingTimerCrane(GetDataRow(notes.children[i]));
                return;
            }
            UpdatingTimerService(GetDataRow(notes.children[i]));
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

    var model = GetDataRow(row);
    UpdatingStatusService(model.URL, model.Status)
}

/*Получить данные строки*/
function GetDataRow(row) {
    var rowDetails = [];
    var collectionInput = row.getElementsByTagName("input");
    for (var i = 0; i < collectionInput.length; i++) {
        var element = collectionInput[i];
        rowDetails[element.id] = element.value;
    }

    return rowDetails;
}

/*Обновить таймер крана*/
function UpdatingTimerCrane(crane) {
    $.ajax({
        type: "GET",
        data: {
            'URL': crane.URL,
            'ActivityTime': crane.ActivityTime,
            'StatusCrane': crane.Status,
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
            'StatusService': internetService.Status,
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
function UpdatingStatusService(url, status, isManualStart = false) {
    $.ajax({
        type: "GET",
        data: {
            url: url,
            statusService: status,
            isManualStart: isManualStart,
        },
        contentType: "application/json; charset=utf-8",
        url: "/Start/UpdateStatusService/",
        success: function (data) {
            debugger;
            if (isManualStart) {
                $('#ManualStart').html(data);
                return;
            }
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
            'StatusCrane': crane.Status,
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
            'StatusService': internetService.Status,
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
        list[i - 2] = GetDataRow(row.children[i]);
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
        list[i - 2] = GetDataRow(row.children[i]);
    }

    for (var i = 0; i < list.length; i++) {
        var element = list[i];
        if (internetService.URL === element.URL && element.ActivityTime === "00:00:00") {
            CheckSites("InternetService", false);
        }
    }
}

function InternetServicesManualStart(button) {
    var divContainer = button.parentElement;
    var data = GetDataRow(divContainer.parentElement);
    button.remove();
    if (button.children[0].innerText === "Старт") {
        divContainer.insertAdjacentHTML('beforeend',
            '<button class="btn btn-warning btn-sm btn-block mt-1" onclick="InternetServicesManualStart(this)"><span>Стоп</span></button>');
        debugger;
        UpdatingStatusService(data.URL, "InWork", true);
        GoToInternetServicesManualStart(data);
    }
    else {
        divContainer.insertAdjacentHTML('beforeend',
            '<button class="btn btn-warning btn-sm btn-block mt-1" onclick="InternetServicesManualStart(this)"><span>Старт</span></button>');
        CloseBrowser(data.TypeService);
    }    
}

function GoToInternetServicesManualStart(data) {
    $.ajax({
        type: "GET",
        data: {
            'URL': data.URL,
            'TypeService': data.TypeService,
        },
        url: "/Start/InternetServicesManualStart"
    });
}

setInterval(function () {
    debugger;
    $.ajax({
        type: "GET",
        url: "/Start/UpdateDataManualStartView",
        success: function (data) {
            $('#ManualStart').html(data);
        }
    });
}, 60000)

function CloseBrowser(typeService) {
    $.ajax({
        type: "GET",
        data: {
            'TypeService': typeService,
        },
        url: "/Start/CloseBrowserManualStart",
        success: function (data) {
            $('#ManualStart').html(data);
        }
    });
}