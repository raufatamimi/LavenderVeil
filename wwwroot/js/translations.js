/* --- Translation Data --- */
const translations = {
  en: {
    // Navigation
    "nav_home": "Home",
    "nav_about": "About",
    "nav_category": "Category",
    "nav_product_details": "Product Details",
    "nav_cart": "Cart",
    "nav_checkout": "Checkout",
    "nav_contact": "Contact",
    "nav_dropdown": "Dropdown",
    "nav_megamenu1": "Megamenu 1",
    "nav_megamenu2": "Megamenu 2",
    // Categories (Example - Add more as needed)
    "cat_abayas": "Abayas",
    "cat_sets": "Sets",
    "cat_dresses": "Dresses",
    "cat_tops": "Tops",
    "cat_skirts": "Skirts",
    "cat_swimwear": "Swimwear",
    "cat_hijab": "Hijab Section",
    "cat_accessories": "Accessories Section",
    "cat_thobes": "Thobes/Robes",
    "cat_prayer_sets": "Prayer Sets",
    // Other common elements
    "customer_support": "Customer Support:",
    "track_order": "Track Order",
    "search_placeholder": "Search for products...",
    "account": "Account",
    "wishlist": "Wishlist",
    "cart": "Cart",
    "footer_copyright": "© Copyright",
    "footer_rights": "All Rights Reserved.",
    "footer_designed_by": "Designed by",
    "footer_terms": "Terms",
    "footer_privacy": "Privacy",
    "footer_cookies": "Cookies",
    "footer_address_label": "Address",
    "footer_phone_label": "Phone:",
    "footer_email_label": "Email:",
    "footer_hours_label": "Hours:",
    "footer_app_store": "App Store",
    "footer_google_play": "Google Play"
  },
  ar: {
    // Navigation
    "nav_home": "الرئيسية",
    "nav_about": "حول",
    "nav_category": "الأقسام", // General category link
    "nav_product_details": "تفاصيل المنتج",
    "nav_cart": "السلة",
    "nav_checkout": "الدفع",
    "nav_contact": "اتصل بنا",
    "nav_dropdown": "قائمة منسدلة",
    "nav_megamenu1": "قائمة ضخمة 1",
    "nav_megamenu2": "قائمة ضخمة 2",
    // Categories
    "cat_abayas": "عبايات",
    "cat_sets": "اطقم",
    "cat_dresses": "فساتين",
    "cat_tops": "ملابس علوية",
    "cat_skirts": "تنانير",
    "cat_swimwear": "ملابس سباحة",
    "cat_hijab": "قسم حجاب",
    "cat_accessories": "قسم اكسسوارات",
    "cat_thobes": "اثواب",
    "cat_prayer_sets": "اطقم صلاة",
    // Other common elements
    "customer_support": "دعم العملاء:",
    "track_order": "تتبع الطلب",
    "search_placeholder": "ابحث عن منتجات...",
    "account": "الحساب",
    "wishlist": "المفضلة",
    "cart": "السلة",
    "footer_copyright": "© حقوق النشر",
    "footer_rights": "جميع الحقوق محفوظة.",
    "footer_designed_by": "تصميم",
    "footer_terms": "الشروط",
    "footer_privacy": "الخصوصية",
    "footer_cookies": "الكوكيز",
    "footer_address_label": "العنوان",
    "footer_phone_label": "الهاتف:",
    "footer_email_label": "البريد الإلكتروني:",
    "footer_hours_label": "الساعات:",
    "footer_app_store": "متجر التطبيقات",
    "footer_google_play": "جوجل بلاي"
  }
};

/**
 * Function to translate content based on selected language
 */
function translateContent(lang) {
  if (!translations[lang]) {
    console.error(`Translation data not found for language: ${lang}`);
    return;
  }

  document.querySelectorAll("[data-translate-key]").forEach(element => {
    const key = element.getAttribute("data-translate-key");
    if (translations[lang][key]) {
      // Handle specific elements like input placeholders differently
      if (element.tagName === "INPUT" && element.placeholder) {
          element.placeholder = translations[lang][key];
      } else {
          element.textContent = translations[lang][key];
      }
    } else {
      console.warn(`Translation key not found for "${key}" in language "${lang}"`);
    }
  });

  // Special handling for footer copyright sitename if needed
  const sitenameElements = document.querySelectorAll(".sitename");
  sitenameElements.forEach(el => {
      // Keep sitename consistent or translate if key exists
      // Example: el.textContent = translations[lang]["sitename_key"] || "Lavender Veil";
      // For now, let's keep it potentially hardcoded or handled by a specific key if added
  });

  // Update static text in JS if necessary (e.g., cart total text)
  // Example: Update cart total text if it's dynamically generated in JS
}


