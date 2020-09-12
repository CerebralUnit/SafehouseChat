/* eslint-disable */
var scrollPos = 100;

let root = document.documentElement;
 

//Custom Scroll Bar
jQuery(window).on('load', function () {

    var $element = $(".scroll-hijack");

    if ($element.length) {
        $element.mCustomScrollbar({
            theme: "dark",
            scrollInertia: 111,
            alwaysShowScrollbar: true,
            mouseWheelPixels: 200,
            setTop: "-999999px",
            callbacks: {
                onScroll: function () {
                    scrollPos = this.mcs.topPct;

                    if (this.mcs.topPct < 50)
                        jQuery('.chat-body').trigger('scrolledTop');

                },
                onTotalScrollBack: function () {
                    jQuery('.chat-body').trigger('scrolledTop');
                }
            }
        }); 
    }
    $element.mCustomScrollbar("scrollTo", "bottom", { scrollInertia: 0 });
    jQuery('.chat-body').trigger('loaded');
})
$('#add-friend').click(function (e) {
    e.preventDefault();
    $('.add-friend').addClass('open');
});

$('.friend-panel-link').click(function (e) {
    e.preventDefault();
    var target = $(this).data('target');

    $('.profile-panel').not(target).removeClass('active');
    $(target).addClass('active');

    $('.friend-panel-link').not(this).removeClass('active');
    $(this).addClass('active');
});

$(document).on('click', '.chat-list .user', function (e) {
    e.preventDefault();

    var left = $(this).offset().left;
    var top = $(this).offset().top;

    var $avatar = $(this).find('.avatar').clone();
    var $username = $(this).find('.user-username').clone();

    $('.popout-avatar').html($avatar).append($username);

    root.style.setProperty('--popout-top', top + "px");
    root.style.setProperty('--popout-left', left + "px");

    $('#profile-popout').removeClass('open').removeClass('from-right');

    setTimeout(function () {
        $('#profile-popout').addClass('open');
    }, 50);
   
    $('.chat-list .user').not(this).removeClass('selected');

    $(this).addClass('selected');
});
$(document).on('click', '.chat-message-author', function (e) {
    e.preventDefault();

    var left = $(this).offset().left + $(this).outerWidth() + $('#profile-popout').outerWidth() + 15;
    var top = $(this).offset().top;

    var $avatar = $(this).parents('.chat-message').find('.avatar').clone();
    var $username = $(this).clone();

    $('.popout-avatar').html($avatar).append($username);

    root.style.setProperty('--popout-top', top + "px");
    root.style.setProperty('--popout-left', left + "px");

    $('#profile-popout').removeClass('open').addClass('from-right');

    setTimeout(function () {
        $('#profile-popout').addClass('open');
    }, 50);

    $('.chat-list .user').not(this).removeClass('selected');

    $(this).addClass('selected');
});


$('.dropdown').click(function () {
    var $target = $($(this).data('target'));

    $target.toggleClass('open');

    $(this).toggleClass('open'); 
});
$('.dropdown').click(function (e) {
    e.stopPropagation();
    $('#profile-popout').removeClass('open'); 
});
$('#group-settings').click(function (e) {
    e.stopPropagation();
    $('#group-settings').removeClass('open');

    $('.dropdown').removeClass('open');
});
let tenorCategories = null;

$('.gif-picker').click(function () {

    if (tenorCategories === null) {
        $.get('/api/tenorcategories').then(function (categories) { 
            tenorCategories = categories;
            showTenorCategories(categories);
        });
    } else {
        showTenorCategories(tenorCategories);
    }
});
function showTenorCategories(categories) {
    $('#tenor-popout').addClass('open');
    $('.tenor-categories').empty();
    categories.map(function (cat) {
        $('.tenor-categories').append(`
            <a class="tenor-category-button" data-tag="${cat.searchTerm}" style="background: url(${cat.image}) center center no-repeat "><label>${cat.searchTerm}</label></a> 
        `);
    });

}
$(document).on('click', '.tenor-category-button', function (e) {
    e.preventDefault();

    var tag = $(this).attr('data-tag');

    $.get('/api/tenorsearch', { q: tag })
        .then(function (results) {
            tenorSearch(results);
        })
});
$('#tenor-search').on('keyup', function () {
    var val = $(this).val();

    $.get('/api/tenorsearch', { q: val })
        .then(function (results) {
            tenorSearch(results);
        })
});
$(document).on('click', '#tenor-back', function (e) {
    e.preventDefault();

    $('#tenor-popout').removeClass('search');
});
function tenorSearch(results) {
  

    $('.tenor-results-wrapper').empty();

    if (!results) {
        $('.tenor-results-wrapper').html('No results.');
        return;
    }

    results.map(function (gif) {

        var size = 234 / gif.width;

        var height = gif.height * size;

        $('.tenor-results-wrapper').append(`
            <a class="tenor-result">
                <video class="gif" autoplay loop preload="auto" src="${gif.url}" width="234" height="${height}" alt="${gif.title}"></video>
            </a> 
        `);
    });

    setTimeout(function () {
        $('#tenor-popout').addClass('search');
    },  250)
    
}

$(document).click(function (e) {
    if (
        !$(e.target).is('#group-settings') &&
        !$(e.target).parents('#group-settings').length 
    )
    {
        $('#group-settings').removeClass('open'); 
        $('.dropdown').removeClass('open'); 
    }

    if (
        !$(e.target).is('#dm-create-popout') &&
        !$(e.target).parents('#dm-create-popout').length 
    ) {
        $('#dm-create-popout').removeClass('open');
        $('.dropdown').removeClass('open');
    }
    
    if (!$(e.target).is('.user') &&
        !$(e.target).is('#profile-popout') &&
        !$(e.target).parents('#profile-popout').length &&
        !$(e.target).parents('.user').length &&
        !$(e.target).is('.chat-message-author') &&
        !$(e.target).parents('.chat-message-author').length
    ) {
        $('#profile-popout').removeClass('open'); 
    }

   
    if (!$(e.target).is('#tenor-popout') && 
        !$(e.target).parents('#tenor-popout').length &&
        !$(e.target).is('.gif-picker') &&
        !$(e.target).parents('.gif-picker').length
    ) {
        $('#tenor-popout').removeClass('open');
    }


    if (!$(e.target).is('#chat-list') && !$(e.target).parents('#chat-list').length) {
        $('.user').removeClass('selected');
    }
});

//Sidebar toggle
$(function(){
    var $trigger = $('[data-toggle="sidebar"]'),
        $body = $('body');

    function toggle(e) {
        e.preventDefault();

        if ($(window).width() < 768) {
            if (!$body.hasClass('sidebar-expanded')) {
                $body.addClass('sidebar-expanded');
            } else {
                $body.removeClass('sidebar-expanded');
            }
        }
    }

    $(window).resize(function(){
        if ($(window).width() > 767) {
            if ($body.hasClass('sidebar-expanded')) {
                $body.removeClass('sidebar-expanded');
            }
        }
    });

    $trigger.click(toggle);
});

//Tooltips
$(function () {
    $('[data-toggle="tooltip"]').tooltip({
        html: true,
        container: 'body'
    });
});

$(function () {
    var particles = $('#particles-js'),
        w = $(window);

    function toggleParticles() {
        if (w.scrollTop() >= $('#home').innerHeight()) {
            particles.hide();
        } else {
            particles.show();
        }
    }

    $(window).on('scroll load', toggleParticles);
});

//Copy invite link to clipboard
$(function(){
    var $element = $('[data-invite-link]'),
        copyElement;

    function copy() {
        var $this = $(this);

        //Create the element
        copyElement = $('<input type="text" />').appendTo('body');

        //Set the link as a value of the input appended earlier
        copyElement.val($this.data('invite-link'));
        //Select the value of the appended input
        copyElement.select();

        //Copy the link
        document.execCommand('copy');

        //Remove element from DOM
        setTimeout(function(){
            copyElement.remove();
        }, 100);
    }

    $element.click(copy);
});
function isSelected(username)
{

    for (var i = 0; i < selectedFriends.length; i++) {
        if (selectedFriends[i].username === username) {
            return true;
        }
    }

    return false;
}
$(document).on('change', '.cbx', function (e) {
    var user = $(this).attr('data-user');

    if (!$(this).is(':checked'))
    {
        for (var i = 0; i < selectedFriends.length; i++)
        {
            if (selectedFriends[i].username === user)
            { 
                selectedFriends.splice(i, 1);

                break;
            } 
        }
    }
    else
    {
        for (var i = 0; i < availableFriends.length; i++)
        {
            if (availableFriends[i].username === user)
            {
                selectedFriends.push(availableFriends[i]); 
                break;
            }
        }
    }

    updateFriendAddTags();
});

var selectedFriends = [];
var availableFriends = [];
var friendFilter = '';

$(document).on('keyup', '#dm-friend-search', function () {
    friendFilter = $(this).val();
    updateFriendAddList();
});
function updateFriendAddTags()
{
    $('#selected-dm-friends .chip').each(function () { $(this).remove();});
    selectedFriends.map(function (selected) {
        $(`
            <div class="chip" data-user="${selected.username}">
                ${selected.username}
                <div class="close"><i class="ri-close-line"></i></div>
            </div>
        `).insertBefore($('#dm-friend-search'));
    });
}
$(document).on('click', '#selected-dm-friends .chip .close', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var text = $(this).parent().attr('data-user');

    $('input[data-user="' + text + '"]').prop('checked', false).trigger('change');

    updateFriendAddTags();
});
$(document).on('click', '#create-dm', function (e) {
    var userIds = [];

    if (!selectedFriends || !selectedFriends.length)
        return;

    selectedFriends.map(function (selected) {
        userIds.push(selected.id);
    });
 
    $.post('/api/createconversation', { userIds: userIds })
        .then(function (response) {

        }); 
});

function updateFriendAddList()
{
    var $list = $('.dm-friend-add-list');

    $list.empty();
    var i = 0;
    availableFriends.map(function (friend) { 
        if (friendFilter && friend.username.toLowerCase().indexOf(friendFilter.toLowerCase()) === -1)
            return;

        if (friend.profilePicture) {
            $list.append(`
                <div class="user">
                    <div class="avatar">
                        <img src="${friend.profilePicture}" />
                    </div>
                    <div>${friend.username}</div>
                    <div>
                        <input type="checkbox" class="cbx" id="cbx-${i}" data-user="${friend.username}" style="display: none;" ${isSelected(friend.username) ? 'checked' : ''}>
                        <label for="cbx-${i}" class="check">
                            <svg width="18px" height="18px" viewBox="0 0 18 18">
                            <path d="M1,9 L1,3.5 C1,2 2,1 3.5,1 L14.5,1 C16,1 17,2 17,3.5 L17,14.5 C17,16 16,17 14.5,17 L3.5,17 C2,17 1,16 1,14.5 L1,9 Z"></path>
                            <polyline points="1 9 7 14 15 4"></polyline>
                            </svg>
                        </label>
                    </div>
                </div>
            `);
        }
        else {
            $list.append(`
                <div class="user">
                    <div class="avatar text-avatar">
                        ${friend.username[0]}
                    </div>
                    <div>${friend.username}</div>
                    <div>
                        <input type="checkbox" class="cbx" id="cbx-${i}" data-user="${friend.username}" style="display: none;" ${isSelected(friend.username) ? 'checked' : ''}>
                        <label for="cbx-${i}" class="check">
                            <svg width="18px" height="18px" viewBox="0 0 18 18">
                            <path d="M1,9 L1,3.5 C1,2 2,1 3.5,1 L14.5,1 C16,1 17,2 17,3.5 L17,14.5 C17,16 16,17 14.5,17 L3.5,17 C2,17 1,16 1,14.5 L1,9 Z"></path>
                            <polyline points="1 9 7 14 15 4"></polyline>
                            </svg>
                        </label>
                    </div>
                </div>
            `);
        }
        i++;
    });

}

$('.add-dm').click(function (e) {
    e.preventDefault();
    
    $('.dm-friend-add-list').empty();

    availableFriends = [];

    $.get('/api/friends').then(function (friends) { 
        availableFriends = friends; 
        updateFriendAddList();
    });
});

//Add to friends
// $(function(){
//     var $element = $('#chat-list > ul > li > a');

//     var a, b, c;

//     function toggleMenu(e){
//         e.preventDefault();
//         var $this = $(this);
//         var menu = $("<ul class='menu'></ul>"),
//             addFriend = $("<li><a href='#'>Add to friend list</a></li>");

//         addFriend.appendTo(menu);

//         $this.closest('li').siblings().find('.menu').remove();

//         if (!$this.closest('li').find('.menu').length) {
//             clearTimeout(a);
//             clearTimeout(b);
//             clearTimeout(c);

//             menu.appendTo($this.closest('li'));
//             c = setTimeout(function(){
//                 $this.closest('li').find('.menu').addClass('menu-open');
//             }, 50);
//         } else {
//             if ($this.closest('li').find('.menu').hasClass('menu-open')) {
//                 $this.closest('li').find('.menu').removeClass('menu-open');

//                 a = setTimeout(function(){
//                     $this.closest('li').find('.menu').remove();
//                 }, 300);
//             }
//         }

//         $(document).click(function (e) {
//             var container = $element;

//             // if the target of the click isn't the container nor a descendant of the container
//             if (!container.is(e.target) && container.has(e.target).length === 0) {
//                 menu.removeClass('menu-open');

//                 a = setTimeout(function(){
//                     menu.remove();
//                 }, 300);
//             }
//         });

//         // $(document).click(function (e) {
//         //     var container = $(".menu");
//         //
//         //     // if the target of the click isn't the container nor a descendant of the container
//         //     if (!container.is(e.target) && container.has(e.target).length === 0) {
//         //         $('body').removeClass('menu-is-open');
//         //         container.removeClass('menu-open');
//         //         container.remove();
//         //     }
//         // });

//         // if ($this.closest('li').siblings().find('.menu').hasClass('menu-open')) {
//         //     $('body').removeClass('menu-is-open');
//         //
//         //     menu.removeClass('menu-open');
//         //
//         //     a = setTimeout(function(){
//         //         menu.remove();
//         //     }, 300);
//         // } else {
//         //     clearTimeout(a);
//         //     clearTimeout(b);
//         //
//         //     menu.appendTo($this);
//         //     $this.find(menu).addClass('menu-open');
//         //
//         //     b = setTimeout(function(){
//         //         $('body').addClass('menu-is-open');
//         //     }, 100);
//         // }


//         // if (!$('body').hasClass('menu-is-open')) {
//         //     clearTimeout(a);
//         //     clearTimeout(b);
//         //
//         //     menu.appendTo($this);
//         //     $this.find(menu).addClass('menu-open');
//         //
//         //     b = setTimeout(function(){
//         //         $('body').addClass('menu-is-open');
//         //     }, 100);
//         // } else {
//         //     $('body').removeClass('menu-is-open');
//         //
//         //     menu.removeClass('menu-open');
//         //
//         //     a = setTimeout(function(){
//         //         menu.remove();
//         //     }, 300);
//         // }
//     }

//     $element.click(toggleMenu);
// });

//Modal popup
$(function () {
    var $trigger = $('[data-start="modal-custom"]'),
        $body = $('body'),
        $close = $('.modal-close');

    var a;

    function toggle(e) {
        e.preventDefault();
        var $this = $(this),
            $target = $this.data('target');

        // if ($($target).hasClass('modal-open')) {
        //     $('.modal-block').removeClass('modal-block-open');
        //     a = setTimeout(function () {
        //         $('.modal-block-wrap').removeClass('modal-open');
        //     }, 300);
        //     $body.removeClass('modal-is-open');
        // } else {
        //     clearTimeout(a);
        //
        //     $($target).addClass('modal-open');
        //     a = setTimeout(function () {
        //         $($target).find('.modal-block').addClass('modal-block-open');
        //     }, 50);
        //     $body.addClass('modal-is-open');
        // }

        if ($body.hasClass('modal-is-open')) {
            $('.modal-block').removeClass('modal-block-open');
            $('.modal-block-wrap').removeClass('modal-open');
            $body.removeClass('modal-is-open');
        }

        clearTimeout(a);

        $($target).addClass('modal-open');
        a = setTimeout(function () {
            $($target).find('.modal-block').addClass('modal-block-open');
        }, 50);
        $body.addClass('modal-is-open');
    }

    $('.modal-block-wrap').click(function (e) {
        var container = $(".modal-block");

        // if the target of the click isn't the container nor a descendant of the container
        if (!container.is(e.target) && container.has(e.target).length === 0) {
            $('.modal-block').removeClass('modal-block-open');
            a = setTimeout(function () {
                $('.modal-block-wrap').removeClass('modal-open');
            }, 300);
            $body.removeClass('modal-is-open');
        }
    });

    $trigger.click(toggle);
    $close.click(function(){
        $('.modal-block').removeClass('modal-block-open');
        a = setTimeout(function () {
            $('.modal-block-wrap').removeClass('modal-open');
        }, 300);
        $body.removeClass('modal-is-open');
    });
});

//Form
$(function () {
    var $formInput = $('input');

    $formInput.on('focus', function () {
        var $this = $(this);

        $this.closest('.form-group').addClass('is-focus');
    });

    $formInput.on('blur', function () {
        var $this = $(this);

        $this.closest('.form-group').removeClass('is-focus');
    });

    $formInput.on('input', function () {
        var $this = $(this);

        if ($this.val().length <= 0) {
            $this.closest('.form-group').removeClass('is-filled');
        } else {
            $this.closest('.form-group').addClass('is-filled');
        }
    });
});

$(function () {
    //particlesJS("particles-js", {
    //    "particles": {
    //        "number": {
    //            "value": 100,
    //            "density": {
    //                "enable": true,
    //                "value_area": 250
    //            }
    //        },
    //        "color": {
    //            "value": "#ffffff"
    //        },
    //        "shape": {
    //            "type": "circle",
    //            "stroke": {
    //                "width": 0,
    //                "color": "#000000"
    //            },
    //            "polygon": {
    //                "nb_sides": 5
    //            },
    //            "image": {
    //                "src": "img/github.svg",
    //                "width": 100,
    //                "height": 100
    //            }
    //        },
    //        "opacity": {
    //            "value": 0.2,
    //            "random": false,
    //            "anim": {
    //                "enable": true,
    //                "speed": 0.2,
    //                "opacity_min": 0,
    //                "sync": false
    //            }
    //        },
    //        "size": {
    //            "value": 2,
    //            "random": true,
    //            "anim": {
    //                "enable": true,
    //                "speed": 2,
    //                "size_min": 0,
    //                "sync": false
    //            }
    //        },
    //        "line_linked": {
    //            "enable": false,
    //            "distance": 150,
    //            "color": "#ffffff",
    //            "opacity": 0.4,
    //            "width": 1
    //        },
    //        "move": {
    //            "enable": true,
    //            "speed": 4,
    //            "direction": "none",
    //            "random": true,
    //            "straight": false,
    //            "out_mode": "out",
    //            "bounce": false,
    //            "attract": {
    //                "enable": false,
    //                "rotateX": 600,
    //                "rotateY": 1200
    //            }
    //        }
    //    },
    //    "interactivity": {
    //        "detect_on": "canvas",
    //        "events": {
    //            "onhover": {
    //                "enable": true,
    //                "mode": "bubble"
    //            },
    //            "onclick": {
    //                "enable": true,
    //                "mode": "push"
    //            },
    //            "resize": true
    //        },
    //        "modes": {
    //            "grab": {
    //                "distance": 400,
    //                "line_linked": {
    //                    "opacity": 1
    //                }
    //            },
    //            "bubble": {
    //                "distance": 83.91608391608392,
    //                "size": 1,
    //                "duration": 3,
    //                "opacity": 1,
    //                "speed": 3
    //            },
    //            "repulse": {
    //                "distance": 200,
    //                "duration": 0.4
    //            },
    //            "push": {
    //                "particles_nb": 4
    //            },
    //            "remove": {
    //                "particles_nb": 2
    //            }
    //        }
    //    },
    //    "retina_detect": true
    //});
});