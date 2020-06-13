import { AjaxLoad, AjaxPost, Noop } from "./util.js";
import { LoadTemplates } from "./util.js";
import { MainWindowViewModel } from "./mainwindow.js";
import { PageSelectorViewModel, pageSelectorRegisterWidget } from "./pageselector.js";
import { CredentialsPageViewModel, credentialsPageRegisterWidget } from "./credentialspage.js"
import { ResourcesPageViewModel, resourcesPageRegisterWidget } from "./resourcespage.js";
import { ResourceLineViewModel, resourceLineRegisterWidget } from "./resourceline.js";
import { UsersPageViewModel, usersPageRegisterWidget } from "./userspage.js";
import { UserLineViewModel, userLineRegisterWidget } from "./userline.js";


function Initialize(templates) {
    let mainWindowVm = new MainWindowViewModel();

    mainWindowVm.credentialsPageVm.userName("test");
    mainWindowVm.credentialsPageVm.password("test");
    mainWindowVm.resourcesPageVm.refresh();

    let credentialsPageWidgetName = credentialsPageRegisterWidget(templates["credentialspage.html"]);
    let resourcesPageWidgetName = resourcesPageRegisterWidget(templates["resourcespage.html"]);
    let usersPageWidgetName = usersPageRegisterWidget(templates["userspage.html"]);
    pageSelectorRegisterWidget(templates["pageselector.html"]);
    resourceLineRegisterWidget(templates["resourceline.html"]);
    userLineRegisterWidget(templates["userline.html"]);

    mainWindowVm.pageSelectorVm().addPage(
        "Credentials",
        credentialsPageWidgetName,
        mainWindowVm.credentialsPageVm
    );

    let defaultPage = mainWindowVm.pageSelectorVm().addPage(
        "Resources",
        resourcesPageWidgetName,
        mainWindowVm.resourcesPageVm,
    );

    mainWindowVm.pageSelectorVm().addPage(
        "Users",
        usersPageWidgetName,
        mainWindowVm.usersPageVm,
    );

    mainWindowVm.pageSelectorVm().selected(defaultPage);

    ko.applyBindings(mainWindowVm);
}

window.PageLoaded = function() {
    // TODO on failure
    function delayedInitialization() {
        LoadTemplates([
            "credentialspage.html",
            "resourcespage.html",
            "pageselector.html",
            "resourceline.html",
            "userspage.html",
            "userline.html",
        ], Initialize, Noop);
    }

    window.setTimeout(delayedInitialization, 2000);
}