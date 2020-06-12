function CredentialsPageViewModel() {
    this.userName = ko.observable("");
    this.password = ko.observable("");
    // this.submitAction = function () {
    //     console.log("TODO1: log in " + this.userName() + " " + this.password());
    // };
}

function credentialsPageRegisterWidget(template, viewModel) {
    let widgetName = "credentials-page-widget";
    ko.components.register(widgetName, {
        viewModel: function () {return viewModel;},
        template: template,
    });
    return widgetName;
}

export { CredentialsPageViewModel, credentialsPageRegisterWidget };