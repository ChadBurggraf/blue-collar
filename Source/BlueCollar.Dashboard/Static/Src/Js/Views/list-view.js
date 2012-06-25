/**
 * Provides a base for list view implementations.
 *
 * @constructor
 */
var ListView = Backbone.View.extend({
    className: 'table table-bordered table-striped table-paged',
    cols: 1,
    tagName: 'table',

    initialize: function(options) {
        this.model.get('Collection').bind('reset', this.render, this);
    },  

    /**
     * Gets an element suitable for displaying an empty list message.
     *
     * @param {String} message The message to display.
     * @return {jQuery} An empty list message element.
     */
    empty: function(message) {
        return $('<tr class="empty"/>').append(
            $('<td/>').attr('colspan', this.cols).append(
                $('<p/>').text(message)));
    },

    /**
     * Gets an element suitable for displaying a loading list message.
     *
     * @return {jQuery} A loading list element.
     */
    loading: function() {
        return $('<tr class="loading"/>').append(
            $('<td/>').attr('colspan', this.cols).append(
                $('<p/>').append(
                    $('<span class="loading"/>'))));
    },

    /**
     * Renders the view.
     *
     * @return {ListView} This instance.
     */
    render: function() {
        var collection = this.model.get('Collection'),
            loading = this.model.get('Loading'),
            tbody;

        this.$el.html(this.template());
        tbody = this.$('tbody');

        if (!loading && this.collection.length > 0) {
            this.renderRows(tbody, collection);
        } else if (loading) { 
            this.html(this.loading());
        } else {
            tbody.html(this.empty('There are no jobs to display.'));
        }

        return this;
    },

    /**
     * Renders the view's row collection.
     *
     * @param {jQuery} tbody The list's tbody element.
     * @param {CollarCollection} collection The collection to render rows for.
     * @return {ListView} This instance.
     */
    renderRows: function(tbody, collection) {
        return this;
    }
});