import { UserLineViewModel } from "./userline.js";

function UsersPageViewModel(backend, credentialsPageVm) {
    let self = this;
    this.backend = backend;
    this.users = ko.observableArray([]);
    this.userEditorVm = ko.observable();
    this.refreshEnabled = ko.observable(true);

    this.editUser = function(userLineVm) {
        self.userEditorVm(userLineVm);
        document.getElementById("userEditorActions").scrollIntoView();
        document.getElementById("userEditorPassword").focus();
    }

    function UpdateUserList(users) {
        self.users(users.map(function (user) {
            let vm = UserLineViewModel.create(user, self.backend, credentialsPageVm);
            vm.openEditor = function() { self.editUser(vm); }
            return vm;
        }));
        self.refreshEnabled(true);
    };

    this.refresh = function () {
        self.refreshEnabled(false);
        self.backend.loadUsers(UpdateUserList, function () {
            self.users([]);
            self.refreshEnabled(true);
        });
        self.userEditorVm(null);
    };

    this.pageActivated = this.refresh;
}

function usersPageRegisterWidget(template) {
    let widgetName = "users-page-widget";
    ko.components.register(widgetName, {
        viewModel: function (params) { return params; },
        template: template,
    });
    return widgetName;
}

export { UsersPageViewModel, usersPageRegisterWidget };