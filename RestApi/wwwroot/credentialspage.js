function UnauthenticatedMainContentViewModel(params) {
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

function unauthenticatedMainContentRegisterWidget(template) {
    let unauthenticatedMainContentWidget = {
        viewModel: UnauthenticatedMainContentViewModel,
        template: template,
    };

    ko.components.register("unauthenticated-main-content-widget", unauthenticatedMainContentWidget);
}

export { UnauthenticatedMainContentViewModel, unauthenticatedMainContentRegisterWidget };