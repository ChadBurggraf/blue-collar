var AreaModel = Backbone.Model.extend({
    defaults: {
        ApplicationName: 'Default',
        Collection: new CollarCollection(),
        Loading: false,
        PageCount: 0,
        PageNumber: 1,
        Search: '',
        TotalCount: 0
    },

    initialize: function(options) {
        this.collection.bind('reset', this.reset, this);
    },

    reset: function() {

    }
});