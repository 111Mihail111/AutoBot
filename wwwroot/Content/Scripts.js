/**Статус сервиса */
var _statusService = {
    Work: 'Work',
    NoWork: 'NoWork',
    InWork: 'InWork',
    InSleeping: 'InSleeping'
};

/**Интервал c авто-запуском интернет-сервисов */
setInterval(function () {
    startWork("#InternetService");
}, 60000)

/**Интервал обновления данных для интернет-сервисов руч. запуска */
setInterval(function () {
    $.ajax({
        type: "GET",
        url: "/Start/UpdateDataManualStartView",
        success: function (data) {
            $('#ManualStart').html(data);
        }
    });

    checkLaunchTimeInternetService();
}, 60000)

/**
 * Начать работу
 * @param {any} containerId Идентификатор контейнера с элементами
 */
function startWork(containerId) {
    var container = $(containerId)[0];
    for (var i = 2; i < container.childElementCount; i++) {

        var modelService = getSelectedRowData(container.children[i]);
        var typeService = modelService.TypeService;

        var activationTime = modelService.ActivationTime;
        if (activationTime != '00:00:00') {
            internetServiceTimerUpdate(typeService, activationTime);
            continue;
        }

        var statusService = modelService.StatusService;
        switch (statusService) {
            case _statusService.NoWork:
            case _statusService.InWork:
            case _statusService.InSleeping:
                continue;
                break;
        }

        var runType = modelService.RunType;
        internetServiceStatusUpdate(typeService, _statusService.InWork, runType);
        startCollection(modelService.URL, typeService, runType);
    }
}

/**
 * Сменить статус
 * @param {any} select Входящий DropDownlList-контрол
 */
function changeStatus(select) {
    var row = select.parentElement.parentElement.parentElement;
    var model = getSelectedRowData(row);

    internetServiceStatusUpdate(model.TypeService, select.value, model.RunType)
}

/**
 * Получить данные выбранной строки
 * @param {any} row Html-строка интернет-сервиса
 */
function getSelectedRowData(row) {
    var rowDetails = [];
    var collectionInput = row.getElementsByTagName("input");

    for (var i = 0; i < collectionInput.length; i++) {
        var element = collectionInput[i];
        rowDetails[element.id] = element.value;
    }

    return rowDetails;
}

/**
 * Обновить таймер интернет-сервиса
 * @param {any} typeService Тип сервиса
 * @param {any} activationTime Время активации
 */
function internetServiceTimerUpdate(typeService, activationTime) {
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: "/Start/InternetServiceTimerUpdate",
        data: {
            typeService: typeService,
            activationTime: activationTime,
        },
        success: function (data) {
            $('#InternetService').html(data);
        }
    });
}

/**
 * Обновить статус интернет-сервиса
 * @param {any} typeService Тип интернет-сервиса
 * @param {any} statusService Статус интернет-сервиса
 * @param {any} runType Тип запуска интернет-сервиса
 */
function internetServiceStatusUpdate(typeService, statusService, runType) {
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: "/Start/InternetServiceStatusUpdate",
        data: {
            typeService: typeService,
            status: statusService,
            runType: runType,
        },
        success: function (data) {
            if (runType === 'Auto') {

                $('#InternetService').html(data);
                return;
            }

            $('#ManualStart').html(data);
        }
    });
}

/**
 * Начать сбор
 * @param {any} url Интернет-адрес
 * @param {any} typeService Тип интернет-сервиса
 * @param {any} runType Тип запуска интернет-сервиса
 */
function startCollection(url, typeService, runType) {
    $.ajax({
        type: "GET",
        url: "/Start/GoToInternetService",
        data: {
            url: url,
            typeService: typeService,
            runType: runType,
        },
        success: function (data) {
            if (runType === 'Auto') {
                $('#InternetService').html(data);
                return;
            }

            $('#ManualStart').html(data);
        }
    });
}

/**
 * Ручной запуск интернет-сервиса
 * @param {any} button Кнопка активирующая событие
 */
function runManualInternetService(button) {
    var divContainer = button.parentElement;
    var modelService = getSelectedRowData(divContainer.parentElement);
    var htmlButton = "<button class='btn btn-warning btn-sm btn-block mt-1' onclick='runManualInternetService(this)'><span>Стоп</span></button>";

    button.remove();
    
    if (button.children[0].innerText != "Старт") {
        divContainer.insertAdjacentHTML('beforeend', htmlButton.replace("Стоп", "Старт"));
        return;
    }

    divContainer.insertAdjacentHTML('beforeend', htmlButton);

    var typeService = modelService.TypeService;
    var runType = modelService.RunType;

    internetServiceStatusUpdate(typeService, _statusService.InWork, runType);
    startCollection(modelService.URL, typeService, runType);
}

/**Проверить время запуска интернет-сервиса */
function checkLaunchTimeInternetService() {
    var divPanel = document.getElementById("ManualStart");
    for (var i = 2; i < divPanel.childElementCount; i++) {

        var modelService = getSelectedRowData(divPanel.children[i]);
        var status = modelService.Status;

        if (status != "InSleeping") {
            continue;
        }

        if (!isLaunchTime(modelService.LaunchTime)) {
            continue;
        }

        var runType = modelService.RunType;
        var typeService = modelService.TypeService;

        internetServiceStatusUpdate(typeService, _statusService.InWork, runType);
        startCollection(modelService.URL, typeService, runType);
    }
}

/**
 * Время запуска
 * @param {any} dateTimeLaunch Дата и время запуска
 */
function isLaunchTime(dateTimeLaunch) {
    var result;
    $.ajax({
        type: "GET",
        async: false,
        data: {
            'dateTimeLaunch': dateTimeLaunch,
        },
        url: "/Start/IsTimeToLaunch",
        success: function (data) {
            result = data;
        }
    });

    return result;
}