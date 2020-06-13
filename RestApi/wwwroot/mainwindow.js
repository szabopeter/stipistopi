import { PageSelectorViewModel, pageSelectorRegisterWidget } from "./pageselector.js";
import { CredentialsPageViewModel, credentialsPageRegisterWidget } from "./credentialspage.js";
import { ResourcesPageViewModel, resourcesPageRegisterWidget } from "./resourcespage.js";
import { UsersPageViewModel, usersPageRegisterWidget } from "./userspage.js";
import { Backend } from "./backend.js";

export function MainWindowViewModel() {
    this.isWelcome = false;
    this.backend = Backend();
    this.pageSelectorVm = ko.observable(new PageSelectorViewModel());
    this.credentialsPageVm = new CredentialsPageViewModel(this.backend);
    this.resourcesPageVm = new ResourcesPageViewModel(this.backend);
    this.usersPageVm = new UsersPageViewModel(this.backend);
}
