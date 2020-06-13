import { PageSelectorItemViewModel } from "./pageselectoritem.js";


function PageSelectorViewModel() {
    let self = this;
    this.selectables = ko.observableArray();
    this.selected = ko.observable();
    let add = function (item) {
        self.selectables().push(item);
        if (self.selected() == null)
            self.selected(item);
    };
    this.addPage = function (title, widgetName, viewModel) {
        let pageSelectorItem = new PageSelectorItemViewModel(title, widgetName, viewModel);
        pageSelectorItem.activate = () => {
            self.selected(pageSelectorItem);
            viewModel.pageActivated();
        }
        add(pageSelectorItem);
        return pageSelectorItem;
    };
}

function pageSelectorRegisterWidget(template) {
    let widgetName = "page-selector";
    ko.components.register(widgetName, {
        viewModel: function (params) { return params.vm; },
        template: template,
    });
    return widgetName;
}

export {
    PageSelectorViewModel,
    pageSelectorRegisterWidget,
}
