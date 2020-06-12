function PageSelectorViewModel(pageSelector) {
    if (pageSelector == null) {
        this.selectables = ko.observableArray();
        this.selected = ko.observable();
    } else {
        this.selectables = pageSelector.selectables;
        this.selected = pageSelector.selected;
    }

    this.add = function (item) {
        this.selectables().push(item);
        if (this.selected() == null)
            this.selected(item);
    };
}

function pageSelectorRegisterWidget(template) {
    ko.components.register("page-selector", {
        viewModel: PageSelectorViewModel,
        template: template,
    });
}

export {
    PageSelectorViewModel,
    pageSelectorRegisterWidget,
}
