﻿/**
 * Provides a base for list view implementations.
 *
 * @constructor
 */
var ListView = Backbone.View.extend({
    className: 'table table-bordered table-striped table-paged',
    cols: 1,
    tagName: 'table',

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        this.model.bind('change:Loading', this.render, this);
        this.model.get('Collection').bind('reset', this.render, this);
    },  

    /**
     * Handles a row's display event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    display: function(sender, args) {
        this.trigger('display', this, args);
    },

    /**
     * Handles a row's edit event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    edit: function(sender, args) {
        this.trigger('edit', this, args);
    },

    /**
     * Gets an element suitable for displaying an empty list message.
     *
     * @return {jQuery} An empty list message element.
     */
    empty: function() {
        return $('<tr class="empty"/>').append(
            $('<td/>').attr('colspan', this.cols).append(
                $('<p/>').text(this.emptyMessage())));
    },

    /**
     * Gets the message to display in the empty element.
     *
     * @return {String} An empty message.
     */
    emptyMessage: function() {
        return 'There are no jobs to display.';
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

        if (!loading && collection.length > 0) {
            this.renderRows(tbody, collection);
        } else if (loading) { 
            tbody.html(this.loading());
        } else {
            tbody.html(this.empty());
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
    },

    /**
     * Handles a row's signal event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    signal: function(sender, args) {
        this.trigger('signal', this, args);
    }
});