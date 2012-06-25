/**
 * Manages the list search view.
 *
 * @constructor
 * @extends {FormView}
 */
var SearchView = FormView.extend({
    className: 'form-search',
    template: _.template($('#search-template').html()),

    /**
     * De-serializes the given attributes hash into this view's form fields.
     *
     * @param {Object} attributes A hash of attribute values to fill this instance with.
     * @return {FormView} This instance.
     */
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

    /**
     * Focuses the first element in the form.
     */
    focus: function() {
        this.$('input[name="q"]').focus();
        return this;
    },

    /**
     * Serializes the form.
     *
     * @return {Object} The serialized form attributes.
     */
    serialize: function() {
        return {Search: this.$('input[name="q"]').val()};
    }
});