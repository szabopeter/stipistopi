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
    function delayedInitialization() {
        LoadTemplates([
            "credentialspage.html",
            "resourcespage.html",
            "pageselector.html",
            "resourceline.html",
            "userspage.html",
            "userline.html",
        ], Initialize, function(filename) {
            let load_errors = document.getElementById("load_errors");
            load_errors.style = "display: block;";
            load_errors.innerHTML += "Could not load " + filename + ". You can try clearing the cache and reloading the page. <br />";
            document.getElementById("app_loading").style = "display: none;";
        });
    }

    window.setTimeout(delayedInitialization, 2000);
}