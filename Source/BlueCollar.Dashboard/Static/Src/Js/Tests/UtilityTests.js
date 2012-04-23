module('Utility');

test('DateIsASPNET', function() {
    equal(Date.isASPNET('\/Date(1240718400000)\/'), true);
    equal(Date.isASPNET(null), false);
    equal(Date.isASPNET(undefined), false);
    equal(Date.isASPNET('not a date'), false);
});

test('DateParseFail', function() {
    ok(_.isNull(Date.parse('not a date')), 'Date is not null.');
});

test('DateParseASPNET', function() {
    var date = Date.parseASPNET('\/Date(1240718400000)\/'),
        expected = Date.parse('2009-04-25 9:00 PM');

    ok(typeof date !== 'undefined' && date !== null);
    equal(date.toString(), expected.toString());
    equal(Date.parseASPNET(), null);
    equal(Date.parseASPNET('gibberish'), null);
});

test('DateToRelativeString', function() {
    equal(new Date().add({seconds: -1}).toRelativeString(), 'a second ago');
    equal(new Date().add({seconds: -37}).toRelativeString(), '37 seconds ago');
    equal(new Date().add({minutes: -1}).toRelativeString(), 'a minute ago');
    equal(new Date().add({minutes: -12}).toRelativeString(), '12 minutes ago');
    equal(new Date().add({hours: -1}).toRelativeString(), 'an hour ago');
    equal(new Date().add({hours: -6}).toRelativeString(), '6 hours ago');
    equal(new Date().add({days: -1}).toRelativeString(), 'yesterday');
    equal(new Date().add({days: -3}).toRelativeString(), '3 days ago');
    equal(new Date().add({days: -7}).toRelativeString(), 'a week ago');
    equal(new Date().add({days: -14}).toRelativeString(), '2 weeks ago');
    
    var month = new Date().add({months: -1}).toRelativeString();
    ok(month =='4 weeks ago' || month == 'a month ago');

    equal(new Date().add({months: -4}).toRelativeString(), '4 months ago');
    equal(new Date().add({years: -1}).toRelativeString(), 'a year ago');
    equal(new Date().add({years: -10}).toRelativeString(), '10 years ago');
});

test('NumberToAbbreviatedString', function() {
    equal(new Number(123).toAbbreviatedString(), '123');
    equal(new Number(1024).toAbbreviatedString(), '1K');
    equal(new Number(1400).toAbbreviatedString(), '1.4K');
    equal(new Number(1450).toAbbreviatedString(), '1.5K');
    equal(new Number(125000).toAbbreviatedString(), '125K');
    equal(new Number(1050000).toAbbreviatedString(), '1.1M');
    equal(new Number(2345002934).toAbbreviatedString(), '2.3B');
    equal(new Number(2998).toAbbreviatedString(), '2.9K');
});

test('StringIsvalidIdentifier', function() {
    equal('abc'.isValidIdentifier(), true);
    equal('$abc'.isValidIdentifier(), true);
    equal('_abc123'.isValidIdentifier(), true);
    equal('a.b'.isValidIdentifier(), false);
    equal('1two'.isValidIdentifier(), false);
});

test('StringSplitAndTrim', function() {
    var arr = $.splitAndTrim('scheduled\n', '\n');
    equal(arr.length, 1);

    arr = $.splitAndTrim('scheduled', '\n');
    equal(arr.length, 1);

    arr = $.splitAndTrim('scheduled\n\r*', '\n');
    equal(arr.length, 2);
});