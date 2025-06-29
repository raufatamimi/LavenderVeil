/* Category Translations for Arabic */
document.addEventListener('DOMContentLoaded', function() {
  // Get current language
  const currentLang = document.documentElement.getAttribute('lang') || 'en';
  
  // Update category page with Arabic categories if needed
  if (currentLang === 'ar') {
    updateCategories();
  }
  
  // Listen for language changes
  document.addEventListener('languageChanged', function(e) {
    if (e.detail.language === 'ar') {
      updateCategories();
    } else {
      // Restore English categories if needed
      // This would be implemented if we need to switch back to original categories
    }
  });
  
  function updateCategories() {
    // Map of category translations
    const categoryMap = {
      'Clothing': 'عبايات',
      'Electronics': 'اطقم',
      'Home & Kitchen': 'فساتين',
      'Beauty & Personal Care': 'ملابس علوية',
      'Sports & Outdoors': 'تنانير',
      'Books': 'ملابس سباحة',
      'Toys & Games': 'قسم حجاب'
    };
    
    // Additional categories to add
    const additionalCategories = [
      'قسم اكسسوارات',
      'اثواب',
      'اطقم صلاة'
    ];
    
    // Update existing category names
    const categoryElements = document.querySelectorAll('.category-item');
    categoryElements.forEach(element => {
      const categoryName = element.textContent.trim();
      if (categoryMap[categoryName]) {
        element.textContent = categoryMap[categoryName];
      }
    });
    
    // Add additional categories if needed
    // This would be implemented if we need to dynamically add new category elements
  }
});
