// ===============================================
// MODERN KEPENEK WEBSITE - ENHANCED JAVASCRIPT
// ===============================================

class ModernKepenkWebsite {
    constructor() {
        this.init();
        this.bindEvents();
        this.setupObservers();
    }

    init() {
        // Initialize all components when DOM is ready
        document.addEventListener('DOMContentLoaded', () => {
            this.initializeComponents();
            this.setupAccessibility();
            this.initializeAnimations();
            this.setupPerformanceOptimizations();
        });

        // Initialize on window load for images and external resources
        window.addEventListener('load', () => {
            this.finalizeInitialization();
        });
    }

    initializeComponents() {
        this.heroSlider = new HeroSlider();
        this.scrollAnimations = new ScrollAnimations();
        this.mobileMenu = new MobileMenu();
        this.contactForm = new ContactForm();
        this.lazyLoader = new LazyLoader();
        this.stickyHeader = new StickyHeader();
        this.smoothScroll = new SmoothScroll();
        this.backToTop = new BackToTop();
        this.productGallery = new ProductGallery();
        this.productTabs = new ProductTabs();
        this.searchFunctionality = new SearchFunctionality();
        this.notificationSystem = new NotificationSystem();
    }

    bindEvents() {
        // Phone call tracking
        document.addEventListener('click', (e) => {
            if (e.target.closest('a[href^="tel:"]')) {
                this.trackEvent('phone_call', 'contact', e.target.href.replace('tel:', ''));
            }
        });

        // WhatsApp tracking
        document.addEventListener('click', (e) => {
            if (e.target.closest('a[href*="wa.me"]')) {
                this.trackEvent('whatsapp_click', 'contact', 'whatsapp');
            }
        });

        // Service card clicks
        document.addEventListener('click', (e) => {
            const serviceCard = e.target.closest('.service-card');
            if (serviceCard) {
                const serviceName = serviceCard.querySelector('.service-title')?.textContent;
                this.trackEvent('service_interest', 'engagement', serviceName);
            }
        });
    }

    setupObservers() {
        // Intersection Observer for animations
        this.intersectionObserver = new IntersectionObserver(
            (entries) => this.handleIntersection(entries),
            {
                threshold: 0.1,
                rootMargin: '0px 0px -50px 0px'
            }
        );

        // Performance Observer for monitoring
        if ('PerformanceObserver' in window) {
            this.performanceObserver = new PerformanceObserver((list) => {
                list.getEntries().forEach((entry) => {
                    if (entry.entryType === 'navigation') {
                        this.trackEvent('page_load_time', 'performance', Math.round(entry.loadEventEnd));
                    }
                });
            });
            this.performanceObserver.observe({ entryTypes: ['navigation'] });
        }
    }

    handleIntersection(entries) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate-in');

                // Stagger animations for grid items
                const parent = entry.target.parentElement;
                if (parent?.classList.contains('advantages-grid') ||
                    parent?.classList.contains('services-grid') ||
                    parent?.classList.contains('products-grid')) {

                    const siblings = Array.from(parent.children);
                    const index = siblings.indexOf(entry.target);
                    entry.target.style.animationDelay = `${index * 0.1}s`;
                }

                // Animate counters
                //if (entry.target.classList.contains('stat-number') ||
                //    entry.target.classList.contains('experience-number')) {
                //    this.animateCounter(entry.target);
                //}

                this.intersectionObserver.unobserve(entry.target);
            }
        });
    }

    setupAccessibility() {
        // Skip link
        const skipLink = document.createElement('a');
        skipLink.href = '#main-content';
        skipLink.className = 'skip-link sr-only';
        skipLink.textContent = 'Ana içeriğe geç';
        skipLink.style.cssText = `
            position: absolute;
            top: -40px;
            left: 6px;
            background: #000;
            color: #fff;
            padding: 8px;
            text-decoration: none;
            border-radius: 4px;
            z-index: 10000;
            transition: top 0.3s ease;
        `;

        skipLink.addEventListener('focus', () => {
            skipLink.style.top = '6px';
        });

        document.body.insertBefore(skipLink, document.body.firstChild);

        // Keyboard navigation support
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Tab') {
                document.body.classList.add('keyboard-navigation');
            }
            if (e.key === 'Escape') {
                this.closeAllModals();
            }
        });

        document.addEventListener('mousedown', () => {
            document.body.classList.remove('keyboard-navigation');
        });
    }

    initializeAnimations() {
        // Add animate elements to observer
        const animatedElements = document.querySelectorAll(`
            .advantage-card,
            .service-card,
            .product-card,
            .testimonial-card,
            .area-item,
            .section-header,
            .company-intro,
            .emergency-content,
            .stat-number,
            .experience-number
        `);

        animatedElements.forEach(el => {
            el.classList.add('animate-element');
            this.intersectionObserver.observe(el);
        });
    }

    setupPerformanceOptimizations() {
        // Debounced scroll handler
        let scrollTimer = null;
        const handleScroll = () => {
            if (scrollTimer !== null) {
                clearTimeout(scrollTimer);
            }
            scrollTimer = setTimeout(() => {
                document.body.classList.remove('scrolling');
            }, 150);
            document.body.classList.add('scrolling');
        };

        window.addEventListener('scroll', this.throttle(handleScroll, 16), { passive: true });

        // Debounced resize handler
        let resizeTimer = null;
        const handleResize = () => {
            if (resizeTimer !== null) {
                clearTimeout(resizeTimer);
            }
            resizeTimer = setTimeout(() => {
                window.dispatchEvent(new CustomEvent('resizeend'));
            }, 250);
        };

        window.addEventListener('resize', handleResize);
    }

    finalizeInitialization() {
        // Remove preloader
        const preloader = document.querySelector('.preloader');
        if (preloader) {
            preloader.classList.add('fade-out');
            setTimeout(() => preloader.remove(), 500);
        }

        // Initialize lazy loading for images
        this.lazyLoader.init();
    }

    animateCounter(element) {
        const originalText = element.textContent || '';

        // Boş veya sadece NaN içeriyorsa dur
        if (!originalText || originalText.includes('NaN')) {
            element.textContent = '0';
            return;
        }

        // Sadece rakamları al
        const numbers = originalText.match(/\d+/);
        if (!numbers || !numbers[0]) {
            return; // Sayı yoksa animasyon yapma
        }

        const target = parseInt(numbers[0]);
        if (isNaN(target)) {
            return; // NaN ise dur
        }

        // Animasyonu güvenli şekilde çalıştır
        const duration = 2000;
        const increment = target / (duration / 16);
        let current = 0;

        const timer = setInterval(() => {
            current += increment;
            if (current >= target) {
                current = target;
                clearInterval(timer);
            }

            // Orijinal format + yeni sayı
            const newText = originalText.replace(/\d+/, Math.floor(current));
            element.textContent = newText;
        }, 16);
    }
    trackEvent(eventName, category, label, value = 1) {
        // Google Analytics 4 tracking
        if (typeof gtag !== 'undefined') {
            gtag('event', eventName, {
                event_category: category,
                event_label: label,
                value: value
            });
        }

        // Facebook Pixel tracking
        if (typeof fbq !== 'undefined') {
            fbq('track', 'CustomEvent', {
                event_name: eventName,
                event_category: category
            });
        }
    }

    closeAllModals() {
        const modals = document.querySelectorAll('.modal.show, .dropdown.open');
        modals.forEach(modal => {
            modal.classList.remove('show', 'open');
        });
    }

    throttle(func, limit) {
        let inThrottle;
        return function () {
            const args = arguments;
            const context = this;
            if (!inThrottle) {
                func.apply(context, args);
                inThrottle = true;
                setTimeout(() => inThrottle = false, limit);
            }
        };
    }

    debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }
}

// ===============================================
// HERO SLIDER CLASS
// ===============================================

class HeroSlider {
    constructor() {
        this.slides = document.querySelectorAll('.hero-slide');
        this.dots = document.querySelectorAll('.hero-dots .dot');
        this.currentSlide = 0;
        this.slideInterval = null;
        this.isPlaying = true;

        if (this.slides.length > 1) {
            this.init();
        }
    }

    init() {
        this.bindEvents();
        this.startAutoPlay();
        this.setupKeyboardNavigation();
    }

    bindEvents() {
        // Dot navigation
        this.dots.forEach((dot, index) => {
            dot.addEventListener('click', () => {
                this.goToSlide(index);
                this.resetAutoPlay();
            });
        });

        // Pause on hover
        const slider = document.querySelector('.hero-slider');
        if (slider) {
            slider.addEventListener('mouseenter', () => this.pauseAutoPlay());
            slider.addEventListener('mouseleave', () => this.resumeAutoPlay());
        }

        // Touch/swipe support
        this.setupTouchEvents();
    }

    setupTouchEvents() {
        let startX = 0;
        let endX = 0;

        const slider = document.querySelector('.hero-slider');
        if (!slider) return;

        slider.addEventListener('touchstart', (e) => {
            startX = e.touches[0].clientX;
        }, { passive: true });

        slider.addEventListener('touchend', (e) => {
            endX = e.changedTouches[0].clientX;
            this.handleSwipe(startX, endX);
        }, { passive: true });
    }

    handleSwipe(startX, endX) {
        const threshold = 50;
        const diff = startX - endX;

        if (Math.abs(diff) > threshold) {
            if (diff > 0) {
                this.nextSlide();
            } else {
                this.previousSlide();
            }
            this.resetAutoPlay();
        }
    }

    setupKeyboardNavigation() {
        document.addEventListener('keydown', (e) => {
            if (e.key === 'ArrowLeft') {
                this.previousSlide();
                this.resetAutoPlay();
            } else if (e.key === 'ArrowRight') {
                this.nextSlide();
                this.resetAutoPlay();
            }
        });
    }

    goToSlide(index) {
        this.slides.forEach(slide => slide.classList.remove('active'));
        this.dots.forEach(dot => dot.classList.remove('active'));

        this.slides[index].classList.add('active');
        if (this.dots[index]) this.dots[index].classList.add('active');

        this.currentSlide = index;
    }

    nextSlide() {
        const next = (this.currentSlide + 1) % this.slides.length;
        this.goToSlide(next);
    }

    previousSlide() {
        const prev = this.currentSlide === 0 ? this.slides.length - 1 : this.currentSlide - 1;
        this.goToSlide(prev);
    }

    startAutoPlay() {
        if (this.slides.length <= 1) return;
        this.slideInterval = setInterval(() => {
            if (this.isPlaying) {
                this.nextSlide();
            }
        }, 5000);
    }

    pauseAutoPlay() {
        this.isPlaying = false;
    }

    resumeAutoPlay() {
        this.isPlaying = true;
    }

    resetAutoPlay() {
        this.pauseAutoPlay();
        clearInterval(this.slideInterval);
        setTimeout(() => {
            this.resumeAutoPlay();
            this.startAutoPlay();
        }, 1000);
    }
}

// ===============================================
// SCROLL ANIMATIONS CLASS
// ===============================================

class ScrollAnimations {
    constructor() {
        this.init();
    }

    init() {
        // Parallax effect for hero
        this.setupParallax();

        // Progress indicator
        this.setupProgressIndicator();
    }

    setupParallax() {
        const parallaxElements = document.querySelectorAll('.hero-bg, .cta-section::before');

        const updateParallax = () => {
            const scrolled = window.pageYOffset;
            parallaxElements.forEach(element => {
                const rate = scrolled * -0.3;
                element.style.transform = `translate3d(0, ${rate}px, 0)`;
            });
        };

        window.addEventListener('scroll', this.throttle(updateParallax, 16), { passive: true });
    }

    setupProgressIndicator() {
        const progressBar = document.createElement('div');
        progressBar.className = 'scroll-progress';
        progressBar.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 0%;
            height: 3px;
            background: linear-gradient(90deg, #DC2626, #EA580C);
            z-index: 9999;
            transition: width 0.1s ease;
        `;
        document.body.appendChild(progressBar);

        const updateProgress = () => {
            const scrollTop = window.pageYOffset;
            const docHeight = document.documentElement.scrollHeight - window.innerHeight;
            const progress = (scrollTop / docHeight) * 100;
            progressBar.style.width = `${Math.min(progress, 100)}%`;
        };

        window.addEventListener('scroll', this.throttle(updateProgress, 16), { passive: true });
    }

    throttle(func, limit) {
        let inThrottle;
        return function () {
            const args = arguments;
            const context = this;
            if (!inThrottle) {
                func.apply(context, args);
                inThrottle = true;
                setTimeout(() => inThrottle = false, limit);
            }
        };
    }
}

// ===============================================
// MOBILE MENU CLASS
// ===============================================

class MobileMenu {
    constructor() {
        this.toggle = document.getElementById('mobileMenuToggle');
        this.menu = document.getElementById('navbarMenu');
        this.init();
    }

    init() {
        if (!this.toggle || !this.menu) return;

        this.bindEvents();
        this.setupAnimation();
    }

    bindEvents() {
        this.toggle.addEventListener('click', () => this.toggleMenu());

        // Close on outside click
        document.addEventListener('click', (e) => {
            if (!this.menu.contains(e.target) && !this.toggle.contains(e.target)) {
                this.closeMenu();
            }
        });

        // Close on escape key
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape' && this.menu.classList.contains('active')) {
                this.closeMenu();
            }
        });

        // Close on window resize
        window.addEventListener('resize', () => {
            if (window.innerWidth > 768) {
                this.closeMenu();
            }
        });

        // Close on menu link click
        this.menu.querySelectorAll('a').forEach(link => {
            link.addEventListener('click', () => this.closeMenu());
        });
    }

    setupAnimation() {
        this.toggle.addEventListener('click', () => {
            const spans = this.toggle.querySelectorAll('span');
            spans.forEach((span, index) => {
                span.style.transform = this.menu.classList.contains('active')
                    ? this.getCloseTransform(index)
                    : this.getOpenTransform(index);
            });
        });
    }

    getOpenTransform(index) {
        const transforms = [
            'rotate(45deg) translate(6px, 6px)',
            'opacity: 0',
            'rotate(-45deg) translate(6px, -6px)'
        ];
        return transforms[index] || '';
    }

    getCloseTransform(index) {
        const transforms = [
            'rotate(0) translate(0, 0)',
            'opacity: 1',
            'rotate(0) translate(0, 0)'
        ];
        return transforms[index] || '';
    }

    toggleMenu() {
        this.menu.classList.toggle('active');
        this.toggle.classList.toggle('active');
        document.body.classList.toggle('menu-open');
    }

    closeMenu() {
        this.menu.classList.remove('active');
        this.toggle.classList.remove('active');
        document.body.classList.remove('menu-open');

        // Reset hamburger animation
        const spans = this.toggle.querySelectorAll('span');
        spans.forEach((span, index) => {
            span.style.transform = this.getCloseTransform(index);
        });
    }
}

// ===============================================
// CONTACT FORM CLASS
// ===============================================

class ContactForm {
    constructor() {
        this.forms = document.querySelectorAll('form');
        this.init();
    }

    init() {
        this.forms.forEach(form => this.setupForm(form));
    }

    setupForm(form) {
        const inputs = form.querySelectorAll('input, textarea, select');

        inputs.forEach(input => {
            input.addEventListener('input', () => this.validateField(input));
            input.addEventListener('blur', () => this.validateField(input));

            if (input.type === 'tel') {
                input.addEventListener('input', () => this.formatPhoneNumber(input));
            }
        });

        form.addEventListener('submit', (e) => this.handleSubmit(e, form));
    }

    validateField(field) {
        const errorElement = field.parentNode.querySelector('.error');
        let isValid = true;
        let errorMessage = '';

        // Remove existing error
        if (errorElement) errorElement.remove();
        field.classList.remove('is-invalid');

        // Required field validation
        if (field.hasAttribute('required') && !field.value.trim()) {
            isValid = false;
            errorMessage = 'Bu alan zorunludur.';
        }
        // Email validation
        else if (field.type === 'email' && field.value) {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(field.value)) {
                isValid = false;
                errorMessage = 'Geçerli bir e-posta adresi giriniz.';
            }
        }
        // Phone validation
        else if (field.type === 'tel' && field.value) {
            const phoneRegex = /^[\+]?[0-9\s\-\(\)]{10,}$/;
            if (!phoneRegex.test(field.value)) {
                isValid = false;
                errorMessage = 'Geçerli bir telefon numarası giriniz.';
            }
        }

        if (!isValid) {
            field.classList.add('is-invalid');
            const error = document.createElement('div');
            error.className = 'error';
            error.textContent = errorMessage;
            field.parentNode.appendChild(error);
        }

        return isValid;
    }

    formatPhoneNumber(input) {
        let value = input.value.replace(/\D/g, '');

        if (value.startsWith('90')) {
            value = value.substring(2);
        }

        if (value.length > 0) {
            if (value.length <= 3) {
                value = `(${value}`;
            } else if (value.length <= 6) {
                value = `(${value.slice(0, 3)}) ${value.slice(3)}`;
            } else if (value.length <= 8) {
                value = `(${value.slice(0, 3)}) ${value.slice(3, 6)} ${value.slice(6)}`;
            } else {
                value = `(${value.slice(0, 3)}) ${value.slice(3, 6)} ${value.slice(6, 8)} ${value.slice(8, 10)}`;
            }
        }

        input.value = value;
    }

    handleSubmit(e, form) {
        const inputs = form.querySelectorAll('input, textarea, select');
        let isFormValid = true;

        inputs.forEach(input => {
            if (!this.validateField(input)) {
                isFormValid = false;
            }
        });

        if (!isFormValid) {
            e.preventDefault();
            const firstError = form.querySelector('.is-invalid');
            if (firstError) {
                firstError.scrollIntoView({ behavior: 'smooth', block: 'center' });
                firstError.focus();
            }

            // Show notification
            window.kepenkWebsite?.notificationSystem?.show(
                'Lütfen formdaki hataları düzeltin.',
                'error'
            );
        } else {
            // Track successful form submission
            window.kepenkWebsite?.trackEvent('form_submit', 'contact', form.id || 'contact_form');
        }
    }
}

// ===============================================
// LAZY LOADER CLASS
// ===============================================

class LazyLoader {
    constructor() {
        this.imageObserver = null;
    }

    init() {
        if ('IntersectionObserver' in window) {
            this.setupImageObserver();
            this.observeImages();
        } else {
            // Fallback for older browsers
            this.loadAllImages();
        }
    }

    setupImageObserver() {
        this.imageObserver = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    this.loadImage(entry.target);
                    this.imageObserver.unobserve(entry.target);
                }
            });
        }, {
            rootMargin: '50px 0px'
        });
    }

    observeImages() {
        const lazyImages = document.querySelectorAll('img[data-src]');
        lazyImages.forEach(img => {
            img.classList.add('lazy');
            this.imageObserver.observe(img);
        });
    }

    loadImage(img) {
        const newImg = new Image();
        newImg.onload = () => {
            img.src = img.dataset.src;
            img.classList.remove('lazy');
            img.classList.add('loaded');
            img.removeAttribute('data-src');
        };
        newImg.onerror = () => {
            img.src = '/images/placeholder.jpg'; // Fallback image
            img.classList.remove('lazy');
        };
        newImg.src = img.dataset.src;
    }

    loadAllImages() {
        const lazyImages = document.querySelectorAll('img[data-src]');
        lazyImages.forEach(img => this.loadImage(img));
    }
}

// ===============================================
// STICKY HEADER CLASS
// ===============================================

class StickyHeader {
    constructor() {
        this.header = document.querySelector('.header');
        this.lastScrollY = window.scrollY;
        this.ticking = false;

        if (this.header) {
            this.init();
        }
    }

    init() {
        window.addEventListener('scroll', () => this.requestTick(), { passive: true });
    }

    requestTick() {
        if (!this.ticking) {
            requestAnimationFrame(() => this.updateHeader());
            this.ticking = true;
        }
    }

    updateHeader() {
        const scrollY = window.scrollY;

        // Add scrolled class for styling
        if (scrollY > 100) {
            this.header.classList.add('header-scrolled');
        } else {
            this.header.classList.remove('header-scrolled');
        }

        // Hide/show header based on scroll direction
        if (scrollY > 200) {
            if (scrollY > this.lastScrollY && !this.header.classList.contains('header-hidden')) {
                this.header.classList.add('header-hidden');
            } else if (scrollY < this.lastScrollY && this.header.classList.contains('header-hidden')) {
                this.header.classList.remove('header-hidden');
            }
        } else {
            this.header.classList.remove('header-hidden');
        }

        this.lastScrollY = scrollY;
        this.ticking = false;
    }
}

// ===============================================
// SMOOTH SCROLL CLASS
// ===============================================

class SmoothScroll {
    constructor() {
        this.init();
    }

    init() {
        const scrollLinks = document.querySelectorAll('a[href^="#"]');

        scrollLinks.forEach(link => {
            link.addEventListener('click', (e) => this.handleClick(e, link));
        });
    }

    handleClick(e, link) {
        e.preventDefault();

        const targetId = link.getAttribute('href');
        const targetElement = document.querySelector(targetId);

        if (targetElement) {
            const headerHeight = document.querySelector('.header')?.offsetHeight || 0;
            const targetPosition = targetElement.offsetTop - headerHeight - 20;

            window.scrollTo({
                top: targetPosition,
                behavior: 'smooth'
            });

            // Update URL without triggering scroll
            if (history.pushState) {
                history.pushState(null, null, targetId);
            }
        }
    }
}

// ===============================================
// BACK TO TOP CLASS
// ===============================================

class BackToTop {
    constructor() {
        this.createButton();
    }

    createButton() {
        const button = document.createElement('button');
        button.innerHTML = '<i class="fas fa-arrow-up"></i>';
        button.className = 'back-to-top';
        button.setAttribute('aria-label', 'Sayfa başına dön');

        button.style.cssText = `
            position: fixed;
            bottom: 120px;
            right: 20px;
            width: 50px;
            height: 50px;
            background: linear-gradient(135deg, #DC2626, #B91C1C);
            color: white;
            border: none;
            border-radius: 50%;
            cursor: pointer;
            z-index: 999;
            opacity: 0;
            visibility: hidden;
            transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
            box-shadow: 0 4px 15px rgba(220, 38, 38, 0.3);
        `;

        document.body.appendChild(button);

        this.bindEvents(button);
    }

    bindEvents(button) {
        // Show/hide on scroll
        window.addEventListener('scroll', () => {
            if (window.pageYOffset > 300) {
                button.style.opacity = '1';
                button.style.visibility = 'visible';
            } else {
                button.style.opacity = '0';
                button.style.visibility = 'hidden';
            }
        }, { passive: true });

        // Scroll to top on click
        button.addEventListener('click', () => {
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });

            // Track interaction
            window.kepenkWebsite?.trackEvent('back_to_top', 'navigation', 'click');
        });

        // Hover effects
        button.addEventListener('mouseenter', () => {
            button.style.transform = 'translateY(-3px) scale(1.1)';
            button.style.boxShadow = '0 8px 25px rgba(220, 38, 38, 0.4)';
        });

        button.addEventListener('mouseleave', () => {
            button.style.transform = 'translateY(0) scale(1)';
            button.style.boxShadow = '0 4px 15px rgba(220, 38, 38, 0.3)';
        });
    }
}

// ===============================================
// PRODUCT GALLERY CLASS
// ===============================================

class ProductGallery {
    constructor() {
        this.mainImage = document.querySelector('.main-image img');
        this.thumbnails = document.querySelectorAll('.thumbnail');

        if (this.mainImage && this.thumbnails.length > 0) {
            this.init();
        }
    }

    init() {
        this.bindEvents();
        this.setupImageZoom();
    }

    bindEvents() {
        this.thumbnails.forEach(thumbnail => {
            thumbnail.addEventListener('click', () => this.switchImage(thumbnail));
            thumbnail.addEventListener('keydown', (e) => {
                if (e.key === 'Enter' || e.key === ' ') {
                    e.preventDefault();
                    this.switchImage(thumbnail);
                }
            });
        });
    }

    switchImage(thumbnail) {
        // Update active thumbnail
        this.thumbnails.forEach(thumb => thumb.classList.remove('active'));
        thumbnail.classList.add('active');

        // Update main image
        const newImageSrc = thumbnail.querySelector('img').src;
        const newImageAlt = thumbnail.querySelector('img').alt;

        this.mainImage.src = newImageSrc;
        this.mainImage.alt = newImageAlt;

        // Add transition effect
        this.mainImage.style.opacity = '0';
        setTimeout(() => {
            this.mainImage.style.opacity = '1';
        }, 150);
    }

    setupImageZoom() {
        this.mainImage.addEventListener('click', () => {
            this.openImageModal(this.mainImage.src, this.mainImage.alt);
        });
    }

    openImageModal(src, alt) {
        const modal = document.createElement('div');
        modal.className = 'image-modal';
        modal.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.9);
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 10000;
            opacity: 0;
            transition: opacity 0.3s ease;
        `;

        modal.innerHTML = `
            <div class="modal-content" style="position: relative; max-width: 90%; max-height: 90%;">
                <img src="${src}" alt="${alt}" style="width: 100%; height: auto; border-radius: 8px;">
                <button class="close-btn" style="
                    position: absolute;
                    top: 10px;
                    right: 10px;
                    background: rgba(0, 0, 0, 0.7);
                    color: white;
                    border: none;
                    border-radius: 50%;
                    width: 40px;
                    height: 40px;
                    cursor: pointer;
                    font-size: 18px;
                ">
                    <i class="fas fa-times"></i>
                </button>
            </div>
        `;

        document.body.appendChild(modal);

        // Trigger animation
        setTimeout(() => {
            modal.style.opacity = '1';
        }, 10);

        // Close events
        const closeBtn = modal.querySelector('.close-btn');
        const closeModal = () => {
            modal.style.opacity = '0';
            setTimeout(() => modal.remove(), 300);
        };

        closeBtn.addEventListener('click', closeModal);
        modal.addEventListener('click', (e) => {
            if (e.target === modal) closeModal();
        });

        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape') closeModal();
        });
    }
}

// ===============================================
// PRODUCT TABS CLASS
// ===============================================

class ProductTabs {
    constructor() {
        this.tabButtons = document.querySelectorAll('.tab-button');
        this.tabContents = document.querySelectorAll('.tab-content');

        if (this.tabButtons.length > 0) {
            this.init();
        }
    }

    init() {
        this.tabButtons.forEach(button => {
            button.addEventListener('click', () => this.switchTab(button));
        });
    }

    switchTab(activeButton) {
        const targetTab = activeButton.getAttribute('data-tab');

        // Remove active classes
        this.tabButtons.forEach(btn => btn.classList.remove('active'));
        this.tabContents.forEach(content => content.classList.remove('active'));

        // Add active class to clicked button
        activeButton.classList.add('active');

        // Show corresponding content
        const targetContent = document.getElementById(targetTab);
        if (targetContent) {
            targetContent.classList.add('active');
        }
    }
}

// ===============================================
// SEARCH FUNCTIONALITY CLASS
// ===============================================

class SearchFunctionality {
    constructor() {
        this.searchForm = document.querySelector('.search-form');
        this.searchInput = document.querySelector('.search-input');

        if (this.searchForm && this.searchInput) {
            this.init();
        }
    }

    init() {
        this.bindEvents();
        this.setupAutocomplete();
    }

    bindEvents() {
        let searchTimeout;

        this.searchInput.addEventListener('input', () => {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                const query = this.searchInput.value.trim();
                if (query.length >= 3) {
                    this.performSearch(query);
                }
            }, 300);
        });

        this.searchInput.addEventListener('keydown', (e) => {
            if (e.key === 'Escape') {
                this.searchInput.value = '';
                this.searchInput.blur();
            }
        });
    }

    setupAutocomplete() {
        // Create autocomplete dropdown
        const dropdown = document.createElement('div');
        dropdown.className = 'search-dropdown';
        dropdown.style.cssText = `
            position: absolute;
            top: 100%;
            left: 0;
            right: 0;
            background: white;
            border-radius: 8px;
            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
            max-height: 300px;
            overflow-y: auto;
            z-index: 1000;
            display: none;
        `;

        this.searchForm.style.position = 'relative';
        this.searchForm.appendChild(dropdown);
        this.dropdown = dropdown;
    }

    performSearch(query) {
        // This would typically make an AJAX call to your search endpoint
        console.log('Searching for:', query);

        // Track search
        window.kepenkWebsite?.trackEvent('search', 'engagement', query);
    }
}

// ===============================================
// NOTIFICATION SYSTEM CLASS
// ===============================================

class NotificationSystem {
    constructor() {
        this.createContainer();
    }

    createContainer() {
        this.container = document.createElement('div');
        this.container.className = 'notifications-container';
        this.container.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 10000;
            pointer-events: none;
        `;
        document.body.appendChild(this.container);
    }

    show(message, type = 'info', duration = 5000) {
        const notification = document.createElement('div');
        notification.className = `notification notification-${type}`;
        notification.style.cssText = `
            background: white;
            border-radius: 8px;
            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
            margin-bottom: 12px;
            transform: translateX(400px);
            opacity: 0;
            transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
            pointer-events: auto;
            border-left: 4px solid ${this.getTypeColor(type)};
        `;

        notification.innerHTML = `
            <div style="display: flex; align-items: center; gap: 12px; padding: 16px 20px;">
                <i class="fas fa-${this.getTypeIcon(type)}" style="color: ${this.getTypeColor(type)};"></i>
                <span style="flex: 1; color: #374151;">${message}</span>
                <button class="notification-close" style="
                    background: none;
                    border: none;
                    cursor: pointer;
                    padding: 4px;
                    border-radius: 4px;
                    transition: background 0.2s ease;
                ">
                    <i class="fas fa-times" style="color: #9CA3AF;"></i>
                </button>
            </div>
        `;

        this.container.appendChild(notification);

        // Show animation
        setTimeout(() => {
            notification.style.transform = 'translateX(0)';
            notification.style.opacity = '1';
        }, 100);

        // Close button
        const closeBtn = notification.querySelector('.notification-close');
        closeBtn.addEventListener('click', () => this.remove(notification));

        // Auto remove
        setTimeout(() => {
            if (notification.parentElement) {
                this.remove(notification);
            }
        }, duration);
    }

    remove(notification) {
        notification.style.transform = 'translateX(400px)';
        notification.style.opacity = '0';
        setTimeout(() => {
            if (notification.parentElement) {
                notification.remove();
            }
        }, 300);
    }

    getTypeColor(type) {
        const colors = {
            success: '#10B981',
            error: '#EF4444',
            warning: '#F59E0B',
            info: '#3B82F6'
        };
        return colors[type] || colors.info;
    }

    getTypeIcon(type) {
        const icons = {
            success: 'check-circle',
            error: 'exclamation-circle',
            warning: 'exclamation-triangle',
            info: 'info-circle'
        };
        return icons[type] || icons.info;
    }
}

// ===============================================
// INITIALIZE APPLICATION
// ===============================================

// Initialize the website when DOM is ready
window.kepenkWebsite = new ModernKepenkWebsite();

// Export for external use
window.KepenkWebsite = {
    trackEvent: (name, category, label, value) => {
        window.kepenkWebsite?.trackEvent(name, category, label, value);
    },

    showNotification: (message, type, duration) => {
        window.kepenkWebsite?.notificationSystem?.show(message, type, duration);
    },

    sendWhatsApp: (phoneNumber, message) => {
        const encodedMessage = encodeURIComponent(message);
        const whatsappUrl = `https://wa.me/${phoneNumber}?text=${encodedMessage}`;
        window.open(whatsappUrl, '_blank');
        window.kepenkWebsite?.trackEvent('whatsapp_send', 'contact', phoneNumber);
    },

    copyToClipboard: (text) => {
        if (navigator.clipboard && window.isSecureContext) {
            navigator.clipboard.writeText(text).then(() => {
                window.kepenkWebsite?.notificationSystem?.show('Panoya kopyalandı!', 'success', 2000);
            });
        } else {
            const textArea = document.createElement('textarea');
            textArea.value = text;
            document.body.appendChild(textArea);
            textArea.select();
            document.execCommand('copy');
            document.body.removeChild(textArea);
            window.kepenkWebsite?.notificationSystem?.show('Panoya kopyalandı!', 'success', 2000);
        }
    }
};

// Error handling
window.addEventListener('error', (e) => {
    console.error('JavaScript Error:', e.error);
    // You can send this to your error tracking service
});

window.addEventListener('unhandledrejection', (e) => {
    console.error('Unhandled Promise Rejection:', e.reason);
    // You can send this to your error tracking service
});