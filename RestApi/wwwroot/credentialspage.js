function CredentialsPageViewModel(params) {
    if (params == null) {
        this.userName = ko.observable("");
        this.password = ko.observable("");
    } else {
        this.userName = params.userName;
        this.password = params.password;
    };

    this.submitAction = function () {
        console.log("TODO1: log in " + this.userName() + " " + this.password());
    };
}

function credentialsPageRegisterWidget(template) {
    let widgetName = "credentials-page-widget";
    ko.components.register(widgetName, {
        viewModel: CredentialsPageViewModel,
        template: template,
    });
    return widgetName;
}

export { CredentialsPageViewModel, credentialsPageRegisterWidget };