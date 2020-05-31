import { LoadTemplates } from "./util.js";


export function TemplateManager() {
    let names = {
        resourceLine: "resourceline.hbs",
        resourceList: "resourcelist.hbs",
    };

    let fileNames = []
    for (let key in names) {
        fileNames.push(names[key]);
    }

    let internals = {
        resourceListTemplate: null,
        // templates: {},
    }

    let templateManager = {
        ApplyResourceListTemplate: function (data) { return internals.resourceListTemplate(data); },
        LoadTemplates: function (loadAction) {
            // TODO: on failure
            LoadTemplates(fileNames, loadAction, function () { });
        },
        Load: function (contents) {
            Handlebars.registerPartial("resourceLineInList", contents[names.resourceLine]);
            internals.resourceListTemplate = Handlebars.compile(contents[names.resourceList]);
            // TODO : there is probably no need for this
            // internals.templates = contents;
        },
    };

    return templateManager;
}
