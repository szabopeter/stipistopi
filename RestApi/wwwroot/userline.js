function UserLineViewModel(backend, credentialsPageVm) {
    let self = this;
    this.backend = backend;
    this.credentialsPageVm = credentialsPageVm;
    this.userName = ko.observable("TODO: userName");
    this.password = ko.observable();
    this.role = ko.observable("TODO: role");
    this.openEditor = function () { console.log("Callback is not loaded!"); }
    this.actions = ko.observableArray([]);
    this.changePassword = function () {
        backend.changePassword(self.userName(), self.password(),
            function () {
                let notReallyAnError = { message: "Password of " + self.userName() + " has been changed" };
                backend.errorMessagesVm.addError(notReallyAnError);
                if (self.userName().toUpperCase() == credentialsPageVm.userName().toUpperCase()) {
                    console.log("Updating our local credentials");
                    credentialsPageVm.password(self.password());
                } else {
                    console.log("Changed someone else's password, keeping our own unchanged.");
                }
            },
            function () { }
        );
    };
}

UserLineViewModel.create = function (source, backend, credentialsPageVm) {
    let vm = new UserLineViewModel(backend, credentialsPageVm);
    vm.userName(source.userName);
    vm.role(source.role);
    return vm;
}

function userLineRegisterWidget(template) {
    let widgetName = "userline-widget";
    ko.components.register(widgetName, {
        viewModel: function (params) { return params; },
        template: template,
    });
    return widgetName;
}

export { UserLineViewModel, userLineRegisterWidget }