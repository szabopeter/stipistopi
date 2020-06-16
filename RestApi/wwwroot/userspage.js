import { UserLineViewModel } from "./userline.js";


function UsersPageViewModel(backend) {
    let self = this;
    this.backend = backend;
    this.users = ko.observableArray([]);

    function UpdateUserList(users) {
        self.users(users.map(function(user) {
            let vm = UserLineViewModel.create(user);
            vm.updateActions(self.backend, self.refresh);
            return vm;
        }));
    };

    this.refresh = function () {
        self.backend.loadUsers(UpdateUserList, function() {self.users([])});
    };

    this.pageActivated = this.refresh;
}

function usersPageRegisterWidget(template) {
    let widgetName = "users-page-widget";
    ko.components.register(widgetName, {
        viewModel: function(params) {return params;},
        template: template,
    });
    return widgetName;
}

export { UsersPageViewModel, usersPageRegisterWidget };