/**
 * For view for search.
 */
var SearchFormView = FormView.extend({
    events: {
        "click button[type='reset']": "clear"
    },
    template: _.template($('#search-form-template').html()),

    clear: function() {
        this.trigger('clear', this);
    },

    deserialize: function(attributes) {
        var input = this.$('input[name="q"]').val('');

        if (!_.isUndefined(attributes) && !_.isNull(attributes)) {
            if (_.isString(attributes)) {
                input.val(attributes);
            } else if (!_.isUndefined(attributes.Search)) {
                input.val(attributes.Search);
            } else {
                input.val(attributes.toString());
            }
        }

        return this;
    },

    serialize: function() {
        return this.$('input[name="q"]').val();
    }
});