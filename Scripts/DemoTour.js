$(document).ready(function () {
    var tourSubmitFunc;
    if ($(".awe-region-list:visible").length > 0) {
        tourSubmitFunc = function (e, v, m, f) {
            if (v === -1) {
                $.prompt.prevState();
                return false;
            }
            else if (v === 1) {
                $.prompt.nextState();
                return false;
            }
        },
        tourStates = [
        /* Demo tour steps when regions are enabled -- see below for alternate demo tour when regions are disabled */
	        {
	            html: '<b>Welcome to AuctionWorx Enterprise 2.0</b><p />This is a quick Homepage tour.',
	            buttons: { Next: 1 },
	            focus: 1,
	            position: { container: '#Logo', x: 50, y: -30, width: 300, arrow: 'rm' },
	            submit: tourSubmitFunc
	        },
	        {
	            html: '<b>Main Navigation Menu</b><ul>'
                    + '<li>&bull; HOME – Clicking this menu option will always return users to this homepage.</li>'
                    + '<li>&bull; BROWSE – Rather than searching, you can browse through all listings and filter by category and region.</li>'
                    + '<li>&bull; MY ACCOUNT – Access your account details; including bids and your listings.</li>'
                    + '<li>&bull; SELL – Two step process to create a new auction or fixed price listing or classified ad.</li></ul>',
	            buttons: { Prev: -1, Next: 1 },
	            focus: 1,
	            position: { container: '#AbsTopCenter', x: -275, y: 85, width: 400, arrow: 'lm' },
	            submit: tourSubmitFunc
	        },
	        {
	            html: "<b>Search</b><p />Enter keywords and search all listing titles and descriptions.",
	            buttons: { Prev: -1, Next: 1 },
	            focus: 1,
	            position: { container: '#AbsTopCenter', x: -350, y: 205, width: 300, arrow: 'rm' },
	            submit: tourSubmitFunc
	        },
	        {
	            html: "<b>There is also an advanced search</b><p />Search by more than keywords and filter by price range, listing type, ending soon, and seller ID.",
	            buttons: { Prev: -1, Next: 1 },
	            focus: 1,
	            position: { container: '#AbsTopCenter', x: 0, y: 225, width: 300, arrow: 'rm' },
	            submit: tourSubmitFunc
	        },
	        {
	            html: "<b>This top banner is optional.</b><p />It can be shown on every page or just specific categories.",
	            buttons: { Prev: -1, Next: 1 },
	            focus: 1,
	            position: { container: '#AbsTopCenter', x: -150, y: 190, width: 300, arrow: 'bc' },
	            submit: tourSubmitFunc
	        },
	        {
	            html: "<b>So is this side banner.</b><p />There is also a bottom banner space included in the template (not shown in this demo).",
	            buttons: { Prev: -1, Next: 1 },
	            focus: 1,
	            position: { container: '.LeftBannerContainer', x: 180, y: 50, width: 300, arrow: 'lm' },
	            submit: tourSubmitFunc
	        },
	        {
	            html: '<b>These are the listing categories.</b><p />You can customize all of your categories right in the administration control panel. You can also hide empty categories and hide the listing count (#).',
	            buttons: { Prev: -1, Next: 1 },
	            focus: 1,
	            position: { container: '.awe-category-list:visible', x: 180, y: -98, width: 400, arrow: 'lb' },
	            submit: tourSubmitFunc
	        },
	        {
	            html: '<b>These are “regions”.</b><p />Regions are an optional secondary category for listings.  They can be used for actual regions (i.e. States and Cities) or customized for any other grouping.',
	            buttons: { Prev: -1, Next: 1 },
	            focus: 1,
	            position: { container: '.awe-region-list:visible', x: 180, y: -98, width: 400, arrow: 'lb' },
	            submit: tourSubmitFunc
	        },
	        {
	            html: '<b>This is a “Homepage Featured” listing.</b>',
	            buttons: { Prev: -1, Next: 1 },
	            focus: 1,
	            position: { container: '#HomepageFeaturedList', x: 150, y: 70, width: 300, arrow: 'lt' },
	            submit: tourSubmitFunc
	        },
	        {
	            html: '<b>So is this one.</b><p />“Homepage Featured” status is an option when creating a listing.  If allowing third party sellers, a fee can be assigned to this listing upgrade.',
	            buttons: { Prev: -1, Next: 1 },
	            focus: 1,
	            position: { container: '#HomepageFeaturedList', x: 130, y: 180, width: 400, arrow: 'rb' },
	            submit: tourSubmitFunc
	        },
	        {
	            html: '<b>Current Date and Time</b><p />This displays the time in the same zone as the webserver.  If you are running your auction event in a different time zone, be sure to update the “time zone offset” setting in the administration control panel!',
	            buttons: { Prev: -1, Next: 1 },
	            focus: 1,
	            position: { container: '#Time', x: -350, y: -152, width: 400, arrow: 'rb' },
	            submit: tourSubmitFunc
	        },
	        {
	            html: '<b>Secondary  Menu</b><p />These menu items provide additional information to users.  All menu options, except for the site map, contain custom content that is added directly in the administration control panel.',
	            buttons: { Prev: -1, Next: 1 },
	            focus: 1,
	            position: { container: '#FooterLinks', x: 560, y: -152, width: 300, arrow: 'lb' },
	            submit: tourSubmitFunc
	        },
	        {
	            html: '<b>Click here to sign in and try out the demo for yourself!</b>',
	            buttons: { Done: 2 },
	            focus: 1,
	            position: { container: '#AbsTopLeft', x: 300, y: 90, width: 300, arrow: 'tc' },
	            //position: { container: '#DemoUserLogin', x: -60, y: 40, width: 300, arrow: 'tc' },
	            submit: tourSubmitFunc
	        }
        ];
    } else { // if ($("#RegionList").length > 0)
        tourSubmitFunc = function (e, v, m, f) {
            if (v === -1) {
                $.prompt.prevState();
                return false;
            }
            else if (v === 1) {
                $.prompt.nextState();
                return false;
            }
        },
        tourStates = [
        /* Demo tour steps when regions are disabled -- see above for alternate demo tour when regions are enabled */
            {
                html: '<b>Welcome to AuctionWorx Enterprise 1.3</b><p />This is a quick Homepage tour.',
                buttons: { Next: 1 },
                focus: 1,
                position: { container: '#Logo', x: 300, y: -30, width: 300, arrow: 'lm' },
                submit: tourSubmitFunc
            },
            {
                html: '<b>Main Navigation Menu</b><ul>'
                    + '<li>&bull; HOME – Clicking this menu option will always return users to this homepage.</li>'
                    + '<li>&bull; BROWSE – Rather than searching, you can browse through all listings and filter by category and region.</li>'
                    + '<li>&bull; MY ACCOUNT – Access your account details; including bids and your listings.</li>'
                    + '<li>&bull; SELL – Two step process to create a new auction or fixed price listing or classified ad.</li></ul>',
                buttons: { Prev: -1, Next: 1 },
                focus: 1,
                position: { container: '#AbsTopCenter', x: -120, y: 120, width: 400, arrow: 'lm' },
                submit: tourSubmitFunc
            },
            {
                html: "<b>Search</b><p />Enter keywords and search all listing titles and descriptions.",
                buttons: { Prev: -1, Next: 1 },
                focus: 1,
                position: { container: '#AbsTopCenter', x: -90, y: 215, width: 300, arrow: 'lm' },
                submit: tourSubmitFunc
            },
            {
                html: "<b>There is also an advanced search</b><p />Search by more than keywords and filter by price range, listing type, ending soon, and seller ID.",
                buttons: { Prev: -1, Next: 1 },
                focus: 1,
                position: { container: '#AbsTopCenter', x: 0, y: 230, width: 300, arrow: 'rm' },
                submit: tourSubmitFunc
            },
            {
                html: "<b>This top banner is optional.</b><p />It can be shown on every page or just specific categories.",
                buttons: { Prev: -1, Next: 1 },
                focus: 1,
                position: { container: '#AbsTopCenter', x: -150, y: 190, width: 300, arrow: 'bc' },
                submit: tourSubmitFunc
            },
            {
                html: "<b>So is this side banner.</b><p />There is also a bottom banner space included in the template (not shown in this demo).",
                buttons: { Prev: -1, Next: 1 },
                focus: 1,
                position: { container: '.LeftBannerContainer', x: 180, y: 50, width: 300, arrow: 'lm' },
                submit: tourSubmitFunc
            },
            {
                html: '<b>These are the listing categories.</b><p />You can customize all of your categories right in the administration control panel. You can also hide empty categories and hide the listing count (#).',
                buttons: { Prev: -1, Next: 1 },
                focus: 1,
                position: { container: '#CategoryList', x: 180, y: -90, width: 400, arrow: 'lb' },
                submit: tourSubmitFunc
            }/*,
	    {
	        html: '<b>These are “regions”.</b><p />Regions are an optional secondary category for listings.  They can be used for actual regions (i.e. States and Cities) or customized for any other grouping.',
	        buttons: { Prev: -1, Next: 1 },
	        focus: 1,
	        position: { container: '#RegionList', x: 180, y: -90, width: 400, arrow: 'lb' },
	        submit: tourSubmitFunc
	    }*/,
            {
                html: '<b>This is a “Homepage Featured” listing.</b>',
                buttons: { Prev: -1, Next: 1 },
                focus: 1,
                position: { container: '#HomepageFeaturedList', x: 150, y: 70, width: 300, arrow: 'lt' },
                submit: tourSubmitFunc
            },
            {
                html: '<b>So is this one.</b><p />“Homepage Featured” status is an option when creating a listing.  If allowing third party sellers, a fee can be assigned to this listing upgrade.',
                buttons: { Prev: -1, Next: 1 },
                focus: 1,
                position: { container: '#HomepageFeaturedList', x: 130, y: 180, width: 400, arrow: 'rb' },
                submit: tourSubmitFunc
            },
            {
                html: '<b>Current Date and Time</b><p />This displays the time in the same zone as the webserver.  If you are running your auction event in a different time zone, be sure to update the “time zone offset” setting in the administration control panel!',
                buttons: { Prev: -1, Next: 1 },
                focus: 1,
                position: { container: '#Time', x: -295, y: -152, width: 400, arrow: 'rb' },
                submit: tourSubmitFunc
            },
            {
                html: '<b>Secondary  Menu</b><p />These menu items provide additional information to users.  All menu options, except for the site map, contain custom content that is added directly in the administration control panel.',
                buttons: { Prev: -1, Next: 1 },
                focus: 1,
                position: { container: '#FooterLinks', x: 450, y: -152, width: 300, arrow: 'lb' },
                submit: tourSubmitFunc
            },
            {
                html: '<b>Click here to sign in and try out the demo for yourself!</b>',
                buttons: { Done: 2 },
                focus: 1,
                position: { container: '#AbsTopLeft', x: 300, y: 90, width: 300, arrow: 'tc' },
                //position: { container: '#DemoUserLogin', x: -60, y: 40, width: 300, arrow: 'tc' },
                submit: tourSubmitFunc
            }
        ];
    }

    var addressBar = new String(document.location);
    //alert(addressBar.indexOf("takeTour=true"));
    if (addressBar.indexOf("/?takeTour=true") != -1) {
        $.prompt(tourStates, { opacity: 0.3 });
        //return false;
    };
});
