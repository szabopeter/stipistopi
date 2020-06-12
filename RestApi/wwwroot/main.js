import { AjaxLoad, AjaxPost, Noop } from "./util.js";
import { TemplateManager } from "./templatemanager.js";
import { ResourceActions } from "./resourceactions.js";
import { MainWindowViewModel } from "./mainwindow.js";
import { PageSelectorViewModel, pageSelectorRegisterWidget } from "./pageselector.js";
import { PageSelectorItemViewModel } from "./pageselectoritem.js";
import { CredentialsPageViewModel, credentialsPageRegisterWidget } from "./credentialspage.js"
import { ResourcesPageViewModel, resourcesPageRegisterWidget } from "./resourcespage.js";
import { ResourceLineViewModel, resourceLineRegisterWidget } from "./resourceline.js";
import { Backend } from "./backend.js";

function StipiStopi(resourceActions, userActions, templateManager) {
    let backend = Backend();

    let stipistopi = {
        // Declarations of public methods, definitions will follow below
        /**
         * Called initially when all templates have been loaded
         * @param contents Hashmap of name -> content
         */
        OnTemplatesLoaded: contents => { },
        /**
         * Issue an ajax request to download resources
         */
        DownloadResourceList: () => { },
        /**
         * Issue an ajax request to download users
         */
        DownloadUserList: () => { },
    };

    function extendResource(resource) {
        function getMainAction(resource) {
            let label = resource.isAvailable ? "Lock" : "Release";
            let backendAction = resource.isAvailable ? "lock" : "release";
            let execute = function () {
                backend.sendResourceAction(backendAction, resource.shortName, stipistopi.DownloadResourceList);
            };

            return {
                label: label,
                backendAction: backendAction,
                execute: execute,
                id: -1,
            }
        };

        resource.actions = [getMainAction(resource)];
    }

    function UpdateResourceList(resources) {
        resources.forEach(extendResource);
        resourceActions.update(resources);

        // Update UI
        let resourceList = document.getElementById("resourceList");
        resourceList.innerHTML = templateManager.ApplyResourceListTemplate(resources);
        console.log("Setting resourceList inner html...");
        // console.log(resourceList.innerHTML);
        document.getElementById("buttonRefreshResourceList").addEventListener("click", stipistopi.DownloadResourceList);
    };

    function extendUser(user) {
        user.isEditing = false;
        user.actions = [{
            label: "Edit",
            execute: function() { console.log("TODO: toggle isEditing for the row"); },
            id: -1,
        }];
    }

    function UpdateUserList(users) {
        users.forEach(extendUser);
        let newUser = {
            userName: "",
            isEditing: true,
            roles: [
                { role: "Regular" },
                { role: "Admin" },
            ],
            actions: [{ 
                id: -1, 
                label: "Save", 
                execute: function() {
                    let newUserName = document.getElementById("userName").value;
                    let newUserPassword = document.getElementById("userPassword").value;
                    let newUserRole = document.getElementById("userRole").value;
                    backend.createUserAction(newUserName, newUserPassword, newUserRole, stipistopi.DownloadUserList);
                }
            }],
        };
        users.splice(0, 0, newUser);
        userActions.update(users);

        let userList = document.getElementById("userList");
        userList.innerHTML = templateManager.ApplyUserListTemplate(users);
        document.getElementById("buttonRefreshUserList").addEventListener("click", stipistopi.DownloadUserList);
    }

    function DownloadResourceList() {
        // TODO: on failure
        AjaxLoad("./stipistopi/resources", "json", UpdateResourceList, function () { });
    };

    function DownloadUserList() {
        // TODO: on failure
        AjaxPost("./stipistopi/users", { userName: "test", password: "test" }, UpdateUserList, function () { });
    }

    function OnTemplatesLoaded(contents) {
        console.log("Everything should be loaded by now...");
        // console.log(contents);
        templateManager.Load(contents);
        stipistopi.DownloadResourceList();
        stipistopi.DownloadUserList();
    };

    // Some methods need to be exposed
    stipistopi.OnTemplatesLoaded = OnTemplatesLoaded;
    stipistopi.DownloadResourceList = DownloadResourceList;
    stipistopi.DownloadUserList = DownloadUserList;

    return stipistopi;
}

function PageLoaded() {
    window.resourceActions = ResourceActions();
    // TODO Rename ResourceActions to ActionRegistry
    window.userActions = ResourceActions();
    console.log("window.resourceActions has been set");
    let templateManager = TemplateManager();
    let stipistopi = StipiStopi(window.resourceActions, window.userActions, templateManager);
    templateManager.LoadTemplates(stipistopi.OnTemplatesLoaded);
}

function Initialize() {
    let mainWindowVm = new MainWindowViewModel();
    mainWindowVm.credentialsPageVm.userName("prefill name");
    mainWindowVm.resourcesPageVm.refresh();


    AjaxLoad("./credentialspage.html", "text", function (content) {
        let widgetName = credentialsPageRegisterWidget(content);
        let pageSelectorItem = new PageSelectorItemViewModel(
            "Credentials",
            widgetName,
            mainWindowVm.credentialsPageVm
        );
        pageSelectorItem.activate = () => mainWindowVm.mainContent(pageSelectorItem);
        mainWindowVm.pageSelector.add(pageSelectorItem);
    }, Noop);

    AjaxLoad("./resourcespage.html", "text", function (content) {
        let widgetName = resourcesPageRegisterWidget(content);
        let pageSelectorItem = new PageSelectorItemViewModel(
            "Resources",
            widgetName,
            mainWindowVm.resourcesPageVm,
        );
        pageSelectorItem.activate = () => mainWindowVm.mainContent(pageSelectorItem);
        mainWindowVm.pageSelector.add(pageSelectorItem);
        mainWindowVm.pageSelector.selected(pageSelectorItem);
    }, Noop);


    AjaxLoad("./pageselector.html", "text", function (content) {
        pageSelectorRegisterWidget(content);
        mainWindowVm.pageSelectorVm(mainWindowVm.pageSelector);
    }, Noop);

    AjaxLoad("./resourceline.html", "text", function (template) {
        let widgetName = resourceLineRegisterWidget(template);
        mainWindowVm.componentManager().register_loaded(widgetName);
    }, Noop);

    ko.applyBindings(mainWindowVm);
}

window.PageLoaded = Initialize;
