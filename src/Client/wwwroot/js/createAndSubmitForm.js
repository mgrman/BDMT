window.createAndSubmitForm = function (action, method, args) {
    var form = document.createElement("form");

    form.method = method;

    form.action = action;
    for (var key in args) {
        var inputElem = document.createElement("input");

        inputElem.name = key;
        inputElem.value = args[key];
        form.appendChild(inputElem);
    }

    document.body.appendChild(form);

    form.submit();
}