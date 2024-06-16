Feature: Car Ads Management

Scenario: Create a new car ad
    Given user is an authenticated user
    When user navigates to the create car ad page
    And user submits a new car ad with valid data
    Then the car ad is created successfully

Scenario: Edit an existing car ad
    Given user is an authenticated user
    And user has an existing car ad
    When user navigates to the edit car ad page
    And user submits updated data for the car ad
    Then the car ad is updated successfully

Scenario: Delete an existing car ad
    Given user is an authenticated user
    And user has an existing car ad
    When user navigates to the delete car ad page
    And user confirms the deletion
    Then the car ad is deleted successfully










