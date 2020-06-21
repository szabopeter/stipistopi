function CredentialsPageViewModel(backend) {
    let self = this;
    this.userName = ko.observable("");
    this.password = ko.observable("");
    this.pageActivated = function () { };
    // this.submitAction = function () {
    //     console.log("TODO1: log in " + this.userName() + " " + this.password());
    // };

    function setBackendCredentials() {
        backend.setCredentials(self.userName(), self.password());
        localStorage.credentials = JSON.stringify(
        {
            userName: self.userName(),
            password: self.password(),
        });
    }
    function loadCredentials() {
        let credentials = null;
        try {
            credentials = JSON.parse(localStorage.credentials);
        } catch (e) {
            console.log("Could not parse stored credentials:");
            console.log(e);
            console.log("Stored object was:");
            console.log(localStorage.credentials);
        }
        if (credentials != null && credentials.userName != null && credentials != null) {
            self.userName(credentials.userName);
            self.password(credentials.password);
            backend.setCredentials(credentials.userName, credentials.password);
        }
    }
    loadCredentials();
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