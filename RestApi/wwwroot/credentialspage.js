function CredentialsPageViewModel(backend) {
    let self = this;
    this.userName = ko.observable("");
    this.password = ko.observable("");
    // this.submitAction = function () {
    //     console.log("TODO1: log in " + this.userName() + " " + this.password());
    // };
    function setBackendCredentials() {
        backend.setCredentials(self.userName(), self.password());
    }
    this.userName.subscribe(setBackendCredentials);
    this.password.subscribe(setBackendCredentials);
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