function CredentialsPageViewModel() {
    this.userName = ko.observable("");
    this.password = ko.observable("");
    // this.submitAction = function () {
    //     console.log("TODO1: log in " + this.userName() + " " + this.password());
    // };
}

function credentialsPageRegisterWidget(template) {
    let widgetName = "credentials-page-widget";
    ko.components.register(widgetName, {
        viewModel: function (params) {return params;},
        template: template,
    });
    return widgetName;
}

export { CredentialsPageViewModel, credentialsPageRegisterWidget };