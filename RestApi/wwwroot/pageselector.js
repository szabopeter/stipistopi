function PageSelectorViewModel() {
    this.selectables = ko.observableArray();
    this.selected = ko.observable();
    this.add = function (item) {
        this.selectables().push(item);
        if (this.selected() == null)
            this.selected(item);
    };
}

function pageSelectorRegisterWidget(template, viewModel) {
    let widgetName = "page-selector";
    ko.components.register(widgetName, {
        viewModel: function() {return viewModel;},
        template: template,
    });
    return widgetName;
}

export {
    PageSelectorViewModel,
    pageSelectorRegisterWidget,
}
