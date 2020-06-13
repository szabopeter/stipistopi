import { AjaxLoad, AjaxPost, Noop } from "./util.js";
import { LoadTemplates } from "./util.js";
import { MainWindowViewModel } from "./mainwindow.js";
import { PageSelectorViewModel, pageSelectorRegisterWidget } from "./pageselector.js";
import { PageSelectorItemViewModel } from "./pageselectoritem.js";
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

    {
        let content = templates["credentialspage.html"];
        let widgetName = credentialsPageRegisterWidget(content);
        mainWindowVm.componentManager().register_loaded(widgetName);
        let pageSelectorItem = new PageSelectorItemViewModel(
            "Credentials",
            widgetName,
            mainWindowVm.credentialsPageVm
        );
        pageSelectorItem.activate = () => mainWindowVm.pageSelectorVm().selected(pageSelectorItem);
        mainWindowVm.pageSelectorVm().add(pageSelectorItem);
    }

    {
        let content = templates["resourcespage.html"];
        let widgetName = resourcesPageRegisterWidget(content);
        mainWindowVm.componentManager().register_loaded(widgetName);
        let pageSelectorItem = new PageSelectorItemViewModel(
            "Resources",
            widgetName,
            mainWindowVm.resourcesPageVm,
        );
        pageSelectorItem.activate = () => {
            mainWindowVm.pageSelectorVm().selected(pageSelectorItem);
            mainWindowVm.resourcesPageVm.refresh();
        }
        mainWindowVm.pageSelectorVm().add(pageSelectorItem);
        mainWindowVm.pageSelectorVm().selected(pageSelectorItem);
    }

    {
        let content = templates["userspage.html"];
        let widgetName = usersPageRegisterWidget(content);
        mainWindowVm.componentManager().register_loaded(widgetName);
        let pageSelectorItem = new PageSelectorItemViewModel(
            "Users",
            widgetName,
            mainWindowVm.usersPageVm,
        );
        pageSelectorItem.activate = () => {
            mainWindowVm.pageSelectorVm().selected(pageSelectorItem);
            mainWindowVm.usersPageVm.refresh();
        }
        mainWindowVm.pageSelectorVm().add(pageSelectorItem);
    }

    {
        let content = templates["pageselector.html"];
        let widgetName = pageSelectorRegisterWidget(content);
        mainWindowVm.componentManager().register_loaded(widgetName);
    }

    {
        let template = templates["resourceline.html"];
        let widgetName = resourceLineRegisterWidget(template);
        mainWindowVm.componentManager().register_loaded(widgetName);
    }

    {
        let template = templates["userline.html"];
        let widgetName = userLineRegisterWidget(template);
        mainWindowVm.componentManager().register_loaded(widgetName);
    }

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