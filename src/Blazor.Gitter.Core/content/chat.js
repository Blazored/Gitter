window.chat = {
    getScrollTop: function (id) {
        const el = document.getElementById(id);
        try {
            if (el) {
                return el.scrollTop;
            } else {
                return -1;
            }
        } catch (e) {
            return -1;
        }
    },
    isScrolledToBottom: function (id) {
        const el = document.getElementById(id);
        try {
            if (el) {
                return el.scrollTop >= el.scrollHeight - el.offsetHeight;
            } else {
                return false;
            }
        } catch (e) {
            return false;
        }
    },
    scrollIntoView: function (id) {
        const el = document.getElementById(id);
        try {
            if (el) {
                el.scrollIntoView();
                return true;
            }
            return false;
        } catch(e) {
            return false;
        }
    }
}