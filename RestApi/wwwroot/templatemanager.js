import { LoadTemplates } from "./util.js";


export function TemplateManager() {
    let names = {
        resourceLine: "resourceline.hbs",
        resourceList: "resourcelist.hbs",
        userList: "userlist.hbs",
        userLine: "userline.hbs",
    };

    let fileNames = []
    for (let key in names) {
        fileNames.push(names[key]);
    }

    let internals = {
        resourceListTemplate: null,
        userListTemplate: null,
    }

    let templateManager = {
        ApplyResourceListTemplate: function (data) { return internals.resourceListTemplate(data); },
        ApplyUserListTemplate: function (data) { return internals.userListTemplate(data); },
        LoadTemplates: function (loadAction) {
            // TODO: on failure
            LoadTemplates(fileNames, loadAction, function () { });
        },
        Load: function (contents) {
            Handlebars.registerPartial("resourceLineInList", contents[names.resourceLine]);
            Handlebars.registerPartial("userLineInList", contents[names.userLine]);
            internals.resourceListTemplate = Handlebars.compile(contents[names.resourceList]);
            internals.userListTemplate = Handlebars.compile(contents[names.userList]);
            // TODO : there is probably no need for this
            // internals.templates = contents;
        },
    };

    return templateManager;
}
