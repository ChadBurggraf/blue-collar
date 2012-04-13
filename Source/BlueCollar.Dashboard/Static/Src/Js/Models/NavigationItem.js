/**
 * NavigationItem model.
 */
var NavigationItem = Backbone.Model.extend({});

/**
 * NavigationItem collection.
 */
var NavigationItemList = Backbone.Collection.extend({
    model: NavigationItem,

    getCurrent: function() {
        var item = this.find(function(item) {
            return item.get('Current');
        });

        if (!item && this.length > 0) {
            this.setCurrent(this.at(0).get('Name'));
            item = this.at(0);
        }

        return item || null;
    },

    parse: function(response) {
        var m = [],
            current = this.getCurrent(),
            urlRoot = this.urlRoot || '',
            showCounts = !!this.showCounts,
            i = 1,
            prop;

        response = response || {};

        function push(id, name, count, url) {
            count = count !== null && showCounts ? count || 0 : null;
            m.push({id: id.toString(), Name: name, Count: count, Current: !!(current && current.get('Name') === name), Url: urlRoot + url});
        }

        push(i++, 'Dashboard', null, '#dashboard');
        push(i++, 'Working', response['WorkingCount'], '#working');
        push(i++, 'Queue', response['QueueCount'], '#queue');
        push(i++, 'History', response['HistoryCount'], '#history');
        push(i++, 'Workers', response['WorkerCount'], '#workers');
        push(i++, 'Schedules', response['ScheduleCount'], '#schedules');

        if (this.testLink) {
            push(i++, 'Tests', null, 'test');
        }

        if (!current) {
            m[0].Current = true;
        }

        return m;
    },

    setCurrent: function(name) {
        var item,
            i,
            n;

        for (i = 0, n = this.length; i < n; i++) {
            item = this.at(i);

            if (item.get('Name') === name) {
                item.set({Current: true});
            } else {
                item.set({Current: false});
            }
        }
    }
});