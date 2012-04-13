/**
 * Renders a list of key/value inputs for managing
 * a properties collection.
 */
var PropertiesView = Backbone.View.extend({
    className: "properties",
    events: {
        'focus input': 'focus'
    },
    tagName: "ul",

    createListItem: function(key, value) {
        var li = $('<li/>'),
            keyInput = $('<input type="text" name="Key" class="code key" placeholder="Key" />'),
            valueInput = $('<input type="text" name="Value" class="code value" placeholder="Value" />');

        keyInput.val(key || '');
        valueInput.val(value || '');

        return li.append(keyInput).append(valueInput);
    },

    focus: function(event) {
        var keys = this.$('input[name="Key"]'),
            values = this.$('input[name="Value"]'),
            targetIndex,
            i,
            n,
            m;

        for (i = 0, n = keys.length, m = n; i < n; i++) {
            if (event.target === keys[i] || event.target === values[i]) {
                targetIndex = i;
            } else if (m > 3 && i < m - 1 && !$(keys[i]).val() && !$(values[i]).val()) {
                this.removeListItem(i);   
                --m;
            }
        }

        if (targetIndex === n - 1) {
            $(this.el).append(this.createListItem().hide().fadeIn());
        }
    },

    removeListItem: function(index) {
        var items = this.$('li');

        if (items.length > index) {
            $(items[index]).remove();
        }
    },

    render: function() {
        var el = $(this.el).html(''),
            json = this.model.toJSON(),
            prop,
            i = 0;

        for (prop in json) {
            if (json.hasOwnProperty(prop)) {
                el.append(this.createListItem(prop, json[prop]));
                i++;
            }
        }

        el.append(this.createListItem());
        i++;

        while (i < 3) {
            el.append(this.createListItem());
            i++;
        }

        return this;
    },

    serialize: function() {
        var obj = {},
            keys = this.$('input[name="Key"]'),
            values = this.$('input[name="Value"]'),
            key,
            value,
            i,
            n;

        for (i = 0, n = keys.length; i < n; i++) {
            key = $.trim($(keys[i]).val());
            value = $.trim($(values[i]).val());

            if (key && key.isValidIdentifier()) {
                obj[key] = value;
            }
        }

        return JSON.stringify(obj);
    },

    validate: function(obj) {
        var errors = [],
            keys = this.$('input[name="Key"]'),
            key,
            i,
            n;

        for (i = 0, n = keys.length; i < n; i++) {
            key = $.trim($(keys[i]).val());
            
            if (key && !key.isValidIdentifier()) {
                errors.push(key);
            }
        }

        if (errors.length > 0) {
            return 'The following keys are not valid JavaScript identifiers: ' + errors.join(', ') + '.';
        }
    }
});