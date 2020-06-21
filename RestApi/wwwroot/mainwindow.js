import { PageSelectorViewModel, pageSelectorRegisterWidget } from "./pageselector.js";
import { CredentialsPageViewModel, credentialsPageRegisterWidget } from "./credentialspage.js";
import { ResourcesPageViewModel, resourcesPageRegisterWidget } from "./resourcespage.js";
import { UsersPageViewModel, usersPageRegisterWidget } from "./userspage.js";
import { Backend } from "./backend.js";
import { ErrorMessagesViewModel } from "./errormessages.js";


export function MainWindowViewModel() {
    this.isWelcome = false;
    this.errorMessagesVm = new ErrorMessagesViewModel();
    this.backend = new Backend(this.errorMessagesVm);
    this.pageSelectorVm = ko.observable(new PageSelectorViewModel());
    this.credentialsPageVm = new CredentialsPageViewModel(this.backend);
    this.resourcesPageVm = new ResourcesPageViewModel(this.backend);
    this.usersPageVm = new UsersPageViewModel(this.backend, this.credentialsPageVm);
}
