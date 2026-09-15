// ================= AUTHENTICATION CHECK =================
const currentToken = sessionStorage.getItem("token");

if (!currentToken) {
    const currentPage = window.location.pathname;

    if (
        currentPage.includes("profile.html") ||
        currentPage.includes("index.html")
    ) {
        window.location.href = "login.html";
    }
}
const registerForm = document.getElementById("registerForm");

if (registerForm) {
    registerForm.addEventListener("submit", async function (event) {
        event.preventDefault();

        const name = document.getElementById("name").value.trim();
        const email = document.getElementById("email").value.trim();
        const password = document.getElementById("password").value;
        const message = document.getElementById("message");

        try {
            const response = await fetch("/api/auth/register", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    name: name,
                    email: email,
                    password: password
                })
            });

            const data = await response.json();

            if (response.ok) {
                message.textContent =
                    data.message || "Registration successful.";

                setTimeout(function () {
                    window.location.href = "login.html";
                }, 1000);
            } else {
                message.textContent =
                    data.message || "Registration failed.";
            }
        } catch (error) {
            console.error(error);
            message.textContent =
                "Unable to connect to the API.";
        }
    });
}


const loginForm = document.getElementById("loginForm");

if (loginForm) {
    loginForm.addEventListener("submit", async function (event) {
        event.preventDefault();

        const email = document.getElementById("email").value.trim();
        const password = document.getElementById("password").value;
        const message = document.getElementById("message");

        try {
            const response = await fetch("/api/auth/login", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    email: email,
                    password: password
                })
            });

            const data = await response.json();

            if (response.ok) {
                sessionStorage.setItem("token", data.token);
                sessionStorage.setItem("userId", data.userId);
                sessionStorage.setItem("name", data.name);
                sessionStorage.setItem("email", data.email);

                message.textContent =
                    "Login successful. Redirecting...";

                setTimeout(function () {
                    window.location.href = "profile.html";
                }, 500);
            } else {
                message.textContent =
                    data.message || "Login failed.";
            }
        } catch (error) {
            console.error(error);
            message.textContent =
                "Unable to connect to the API.";
        }
    });
}


const profileDetails =
    document.getElementById("profileDetails");

if (profileDetails) {
    loadProfile();
}


async function loadProfile() {

    const token = sessionStorage.getItem("token")
    const message = document.getElementById("message");
    const profileDetails =
        document.getElementById("profileDetails");

    if (!token) {
        message.textContent = "Please login first.";
        return;
    }

    try {

        const response = await fetch("/api/Profile", {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (!response.ok) {

            if (response.status === 401) {

                sessionStorage.removeItem("token");
                sessionStorage.removeItem("userId");
                sessionStorage.removeItem("name");
                sessionStorage.removeItem("email");

                message.textContent =
                    "Session expired. Please login again.";

                return;
            }

            throw new Error("Failed to load profile.");
        }

        const data = await response.json();

        message.textContent =
            "Profile loaded successfully.";

        profileDetails.innerHTML = `
            <div class="profile-detail-card">
                <span class="profile-detail-label">Name</span>
                <p>${data.name || "Not provided"}</p>
            </div>

            <div class="profile-detail-card">
                <span class="profile-detail-label">Email</span>
                <p>${data.email || "Not provided"}</p>
            </div>

            <div class="profile-detail-card">
                <span class="profile-detail-label">Bio</span>
                <p>${data.bio || "Not provided"}</p>
            </div>

            <div class="profile-detail-card">
                <span class="profile-detail-label">
                    Experience Level
                </span>
                <p>${data.experienceLevel || "Not provided"}</p>
            </div>

            <div class="profile-detail-card">
                <span class="profile-detail-label">
                    Availability
                </span>
                <p>${data.availability || "Not provided"}</p>
            </div>

            <div class="profile-detail-card">
                <span class="profile-detail-label">
                    Session Duration
                </span>
                <p>${data.preferredSessionDuration || "Not provided"}</p>
            </div>
        `;

    } catch (error) {

        console.error(error);

        message.textContent =
            "Unable to load profile.";
    }
}


const editProfileButton =
    document.getElementById("editProfileButton");

const editProfileForm =
    document.getElementById("editProfileForm");

if (editProfileButton && editProfileForm) {

    editProfileButton.addEventListener("click", function () {

        editProfileForm.style.display = "block";

        document.getElementById("bio").value = "";
        document.getElementById("experienceLevel").value = "";
        document.getElementById("availability").value = "";
        document.getElementById("preferredSessionDuration").value = "";
    });
}


const saveProfileButton =
    document.getElementById("saveProfileButton");

if (saveProfileButton) {

    saveProfileButton.addEventListener(
        "click",
        async function () {

            const token =
                sessionStorage.getItem("token");

            const message =
                document.getElementById("message");

            if (!token) {
                message.textContent =
                    "Please login first.";
                return;
            }

            const bio =
                document.getElementById("bio").value.trim();

            const experienceLevel =
                document.getElementById("experienceLevel")
                    .value.trim();

            const availability =
                document.getElementById("availability")
                    .value.trim();

            const preferredSessionDuration =
                document.getElementById(
                    "preferredSessionDuration"
                ).value.trim();

            try {

                const response = await fetch(
                    "/api/Profile",
                    {
                        method: "PUT",
                        headers: {
                            "Content-Type": "application/json",
                            "Authorization": `Bearer ${token}`
                        },
                        body: JSON.stringify({
                            bio: bio,
                            experienceLevel: experienceLevel,
                            availability: availability,
                            preferredSessionDuration:
                                preferredSessionDuration
                                    ? parseInt(
                                        preferredSessionDuration,
                                        10
                                    )
                                    : null
                        })
                    }
                );

                const data = await response.json();

                if (response.ok) {

                    message.textContent =
                        data.message ||
                        "Profile updated successfully.";

                    editProfileForm.style.display =
                        "none";

                    await loadProfile();

                } else {

                    message.textContent =
                        data.message ||
                        "Profile update failed.";
                }

            } catch (error) {

                console.error(error);

                message.textContent =
                    "Unable to connect to the API.";
            }
        }
    );
}


async function loadSkills() {

    const token =
        sessionStorage.getItem("token");

    const skillSelect =
        document.getElementById("skillSelect");

    if (!token || !skillSelect) {
        return;
    }

    try {

        const response = await fetch(
            "/api/Skills",
            {
                method: "GET",
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            }
        );

        if (!response.ok) {
            console.error("Failed to load skills.");
            return;
        }

        const skills = await response.json();

        skillSelect.innerHTML =
            '<option value="">Select a skill</option>';

        skills.forEach(function (skill) {

            const option =
                document.createElement("option");

            option.value = skill.skillId;
            option.textContent = skill.skillName;

            skillSelect.appendChild(option);
        });

    } catch (error) {

        console.error(error);
    }
}


async function loadMySkills() {

    const token =
        sessionStorage.getItem("token");

    const skillsList =
        document.getElementById("mySkillsList");

    if (!token || !skillsList) {
        return;
    }

    try {

        const response = await fetch(
            "/api/UserSkills",
            {
                method: "GET",
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            }
        );

        if (!response.ok) {

            console.error(
                "Failed to load my skills."
            );

            return;
        }

        const skills = await response.json();

        skillsList.innerHTML = "";

        if (skills.length === 0) {

            skillsList.innerHTML = `
                <div class="empty-state">
                    <div class="empty-state-icon">＋</div>
                    <h3>No skills added yet</h3>
                    <p>
                        Add your skills to start connecting
                        with other learners.
                    </p>
                </div>
            `;

            return;
        }

        skills.forEach(function (skill) {

            const skillDiv =
                document.createElement("div");

            skillDiv.className = "skill-card";

            const skillType =
                skill.skillType === "OFFER"
                    ? "I can teach"
                    : "I want to learn";

            const skillTypeClass =
                skill.skillType === "OFFER"
                    ? "offer-badge"
                    : "want-badge";

            skillDiv.innerHTML = `

                <div class="skill-card-top">

                    <div>
                        <h3>${skill.skillName}</h3>

                        <span class="skill-category">
                            ${skill.category || "No category"}
                        </span>
                    </div>

                    <span class="skill-type-badge ${skillTypeClass}">
                        ${skill.skillType}
                    </span>

                </div>

                <div class="skill-card-info">

                    <div class="skill-info-row">
                        <span>Type</span>
                        <strong>${skillType}</strong>
                    </div>

                    <div class="skill-info-row">
                        <span>Level</span>
                        <strong>
                            ${skill.skillLevel || "Not provided"}
                        </strong>
                    </div>

                </div>

                <div class="skill-card-actions">

                    <button
                        type="button"
                        class="secondary-action-button"
                        onclick="editSkill(
                            ${skill.userSkillId},
                            '${skill.skillType}',
                            '${skill.skillLevel || ""}'
                        )">
                        Edit
                    </button>

                    <button
                        type="button"
                        class="danger-action-button"
                        onclick="deleteSkill(
                            ${skill.userSkillId}
                        )">
                        Delete
                    </button>

                </div>
            `;

            skillsList.appendChild(skillDiv);
        });

    } catch (error) {

        console.error(error);
    }
}


async function addSkill() {

    const token =
        sessionStorage.getItem("token");

    if (!token) {
        alert("Please login first.");
        return;
    }

    const skillId =
        document.getElementById("skillSelect").value;

    const skillType =
        document.getElementById("skillType").value;

    const skillLevel =
        document.getElementById("skillLevel").value;

    if (!skillId) {
        alert("Please select a skill.");
        return;
    }

    if (!skillLevel) {
        alert("Please select a skill level.");
        return;
    }

    try {

        const response = await fetch(
            "/api/UserSkills",
            {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify({
                    skillId: parseInt(skillId, 10),
                    skillType: skillType,
                    skillLevel: skillLevel
                })
            }
        );

        const result = await response.json();

        if (!response.ok) {

            alert(
                result.message ||
                "Failed to add skill."
            );

            return;
        }

        alert(
            result.message ||
            "Skill added successfully."
        );

        await loadMySkills();

        document.getElementById(
            "skillSelect"
        ).value = "";

        document.getElementById(
            "skillLevel"
        ).value = "";

    } catch (error) {

        console.error(error);

        alert(
            "Unable to connect to the API."
        );
    }
}


async function editSkill(
    userSkillId,
    currentSkillType,
    currentSkillLevel
) {

    const token =
        sessionStorage.getItem("token");

    if (!token) {
        alert("Please login first.");
        return;
    }

    const skillType =
        prompt(
            "Enter skill type: OFFER or WANT",
            currentSkillType
        );

    if (skillType === null) {
        return;
    }

    const skillLevel =
        prompt(
            "Enter skill level: Beginner, Intermediate or Advanced",
            currentSkillLevel
        );

    if (skillLevel === null) {
        return;
    }

    try {

        const response = await fetch(
            `/api/UserSkills/${userSkillId}`,
            {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify({
                    skillType: skillType,
                    skillLevel: skillLevel
                })
            }
        );

        const result = await response.json();

        if (!response.ok) {

            alert(
                result.message ||
                "Failed to update skill."
            );

            return;
        }

        alert(
            result.message ||
            "Skill updated successfully."
        );

        await loadMySkills();

    } catch (error) {

        console.error(error);

        alert(
            "Unable to connect to the API."
        );
    }
}


async function deleteSkill(userSkillId) {

    const token =
        sessionStorage.getItem("token");

    if (!token) {
        alert("Please login first.");
        return;
    }

    const confirmed =
        confirm(
            "Are you sure you want to delete this skill?"
        );

    if (!confirmed) {
        return;
    }

    try {

        const response = await fetch(
            `/api/UserSkills/${userSkillId}`,
            {
                method: "DELETE",
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            }
        );

        const result = await response.json();

        if (!response.ok) {

            alert(
                result.message ||
                "Failed to delete skill."
            );

            return;
        }

        alert(
            result.message ||
            "Skill deleted successfully."
        );

        await loadMySkills();

    } catch (error) {

        console.error(error);

        alert(
            "Unable to connect to the API."
        );
    }
}


async function loadMatches() {

    const token =
        sessionStorage.getItem("token");

    const matchesMessage =
        document.getElementById("matchesMessage");

    const matchesList =
        document.getElementById("matchesList");

    if (!matchesMessage || !matchesList) {
        return;
    }

    if (!token) {

        matchesMessage.textContent =
            "Please login to view your matches.";

        return;
    }

    try {

        const response = await fetch(
            "/api/Matches",
            {
                method: "GET",
                headers: {
                    "Authorization":
                        "Bearer " + token
                }
            }
        );

        if (!response.ok) {

            matchesMessage.textContent =
                "Unable to load matches.";

            return;
        }

        const matches =
            await response.json();

        matchesList.innerHTML = "";

        if (matches.length === 0) {

            matchesMessage.textContent =
                "No skill matches found.";

            return;
        }

        matchesMessage.textContent =
            "Users who can help you learn:";

        matches.forEach(function (match) {

            const matchDiv =
                document.createElement("div");

            matchDiv.className =
                "match-card";

            matchDiv.innerHTML = `

                <div class="match-card-header">

                    <div class="match-avatar">
                        ${match.name
                    ? match.name
                        .charAt(0)
                        .toUpperCase()
                    : "U"}
                    </div>

                    <div>
                        <h3>${match.name}</h3>

                        <span class="match-subtitle">
                            Skill Match
                        </span>
                    </div>

                </div>

                <div class="match-card-info">

                    <p>
                        <span>Skill</span>
                        <strong>${match.skillName}</strong>
                    </p>

                    <p>
                        <span>Level</span>
                        <strong>
                            ${match.skillLevel || "Not provided"}
                        </strong>
                    </p>

                    <p>
                        <span>Email</span>
                        <strong>${match.email}</strong>
                    </p>

                </div>

                <button
                    type="button"
                    class="request-session-button"
                    onclick="requestSession(
                        ${match.userId},
                        ${match.skillId}
                    )">
                    Request Session
                </button>
            `;

            matchesList.appendChild(matchDiv);
        });

    } catch (error) {

        console.error(error);

        matchesMessage.textContent =
            "Unable to connect to the API.";
    }
}


async function requestSession(
    receiverUserId,
    skillId
) {

    const token =
        sessionStorage.getItem("token");

    if (!token) {
        alert("Please login first.");
        return;
    }

    try {

        const response = await fetch(
            "/api/SessionRequests",
            {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization":
                        "Bearer " + token
                },
                body: JSON.stringify({
                    receiverUserId: receiverUserId,
                    skillId: skillId
                })
            }
        );

        const result =
            await response.json();

        if (!response.ok) {

            alert(
                result.message ||
                "Failed to send session request."
            );

            return;
        }

        alert(
            result.message ||
            "Session request sent successfully."
        );

        await loadMatches();

    } catch (error) {

        console.error(error);

        alert(
            "Unable to connect to the API."
        );
    }
}


async function loadSessionRequests() {

    const token =
        sessionStorage.getItem("token");

    const requestsMessage =
        document.getElementById("requestsMessage");

    const requestsList =
        document.getElementById("requestsList");

    if (!requestsMessage || !requestsList) {
        return;
    }

    if (!token) {

        requestsMessage.textContent =
            "Please login to view session requests.";

        return;
    }

    try {

        const response = await fetch(
            "/api/SessionRequests",
            {
                method: "GET",
                headers: {
                    "Authorization":
                        "Bearer " + token
                }
            }
        );

        if (!response.ok) {

            requestsMessage.textContent =
                "Unable to load session requests.";

            return;
        }

        const requests =
            await response.json();

        requestsList.innerHTML = "";

        if (requests.length === 0) {

            requestsMessage.textContent =
                "No session requests found.";

            return;
        }

        requestsMessage.textContent =
            "Your received session requests:";

        requests.forEach(function (request) {

            const requestDiv =
                document.createElement("div");

            requestDiv.className =
                "request-card";

            let actionButtons = "";

            if (request.status === "Pending") {

                actionButtons = `

                    <div class="request-actions">

                        <button
                            type="button"
                            class="accept-button"
                            onclick="updateSessionRequest(
                                ${request.sessionRequestId},
                                'Accepted'
                            )">
                            Accept
                        </button>

                        <button
                            type="button"
                            class="reject-button"
                            onclick="updateSessionRequest(
                                ${request.sessionRequestId},
                                'Rejected'
                            )">
                            Reject
                        </button>

                    </div>
                `;

            } else if (request.status === "Accepted") {

                actionButtons = `

                    <div class="request-actions">

                        <button
                            type="button"
                            class="schedule-button"
                            onclick="showScheduleForm(
                                ${request.sessionRequestId}
                            )">
                            Schedule Session
                        </button>

                    </div>
                `;
            }

            const statusClass =
                request.status.toLowerCase();

            requestDiv.innerHTML = `

                <div class="request-card-header">

                    <div>

                        <h3>
                            ${request.requesterName}
                        </h3>

                        <span class="request-skill">
                            ${request.skillName}
                        </span>

                    </div>

                    <span class="status-badge ${statusClass}">
                        ${request.status}
                    </span>

                </div>

                <div class="request-card-info">

                    <p>
                        <span>Email</span>
                        <strong>
                            ${request.requesterEmail}
                        </strong>
                    </p>

                    <p>
                        <span>Skill</span>
                        <strong>
                            ${request.skillName}
                        </strong>
                    </p>

                </div>

                <div
                    id="scheduleForm-${request.sessionRequestId}">
                </div>

                ${actionButtons}
            `;

            requestsList.appendChild(requestDiv);
        });

    } catch (error) {

        console.error(error);

        requestsMessage.textContent =
            "Unable to connect to the API.";
    }
}


function showScheduleForm(sessionRequestId) {

    const formContainer =
        document.getElementById(
            "scheduleForm-" + sessionRequestId
        );

    if (!formContainer) {
        return;
    }

    formContainer.innerHTML = `

        <div class="schedule-form">

            <div class="schedule-form-header">

                <div>
                    <h4>Schedule Session</h4>
                    <p>
                        Choose a convenient date and time.
                    </p>
                </div>

            </div>

            <div class="schedule-form-grid">

                <div class="schedule-field">

                    <label>Date</label>

                    <input
                        type="date"
                        id="scheduleDate-${sessionRequestId}"
                    >

                </div>

                <div class="schedule-field">

                    <label>Time</label>

                    <input
                        type="time"
                        id="scheduleTime-${sessionRequestId}"
                    >

                </div>

                <div class="schedule-field">

                    <label>Duration</label>

                    <select
                        id="scheduleDuration-${sessionRequestId}"
                    >

                        <option value="30">
                            30 minutes
                        </option>

                        <option value="45">
                            45 minutes
                        </option>

                        <option value="60">
                            60 minutes
                        </option>

                        <option value="90">
                            90 minutes
                        </option>

                        <option value="120">
                            120 minutes
                        </option>

                    </select>

                </div>

            </div>

            <button
                type="button"
                class="confirm-schedule-button"
                onclick="scheduleSession(
                    ${sessionRequestId}
                )">
                Confirm Schedule
            </button>

        </div>
    `;
}


async function scheduleSession(sessionRequestId) {

    const token =
        sessionStorage.getItem("token");

    if (!token) {
        alert("Please login first.");
        return;
    }

    const date =
        document.getElementById(
            "scheduleDate-" + sessionRequestId
        ).value;

    const time =
        document.getElementById(
            "scheduleTime-" + sessionRequestId
        ).value;

    const duration =
        document.getElementById(
            "scheduleDuration-" + sessionRequestId
        ).value;

    if (!date || !time) {

        alert(
            "Please select date and time."
        );

        return;
    }

    const requestBody = {

        sessionId: 0,

        sessionRequestId:
            sessionRequestId,

        swapRequestId: 0,

        scheduledDate:
            date,

        startTime:
            time + ":00",

        durationMinutes:
            parseInt(duration),

        meetingLink: "",

        teacherCompleted:
            false,

        learnerCompleted:
            false,

        status:
            "SCHEDULED"
    };

    try {

        const response = await fetch(
            "https://localhost:7121/api/Sessions/schedule",
            {
                method: "POST",

                headers: {
                    "Content-Type": "application/json",
                    "Authorization":
                        "Bearer " + token
                },

                body:
                    JSON.stringify(requestBody)
            }
        );

        const resultText =
            await response.text();

        let result;

        try {
            result =
                JSON.parse(resultText);
        } catch {
            result =
                resultText;
        }

        if (!response.ok) {

            console.error(
                "Schedule failed:",
                response.status,
                result
            );

            alert(
                typeof result === "string"
                    ? result
                    : result.message ||
                    "Failed to schedule session."
            );

            return;
        }

        alert(
            result.message ||
            "Session scheduled successfully!"
        );

        await loadSessionRequests();

        if (
            typeof loadUpcomingSessions ===
            "function"
        ) {
            await loadUpcomingSessions();
        }

    } catch (error) {

        console.error(error);

        alert(
            "Unable to connect to the API."
        );
    }
}


async function updateSessionRequest(
    sessionRequestId,
    status
) {

    const token =
        sessionStorage.getItem("token");

    if (!token) {
        alert("Please login first.");
        return;
    }

    try {

        const response = await fetch(
            `/api/SessionRequests/${sessionRequestId}`,
            {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization":
                        "Bearer " + token
                },
                body:
                    JSON.stringify(status)
            }
        );

        const result =
            await response.json();

        if (!response.ok) {

            console.log(
                "Schedule API status:",
                response.status
            );

            console.log(
                "Schedule API response:",
                result
            );

            alert(
                JSON.stringify(result)
            );

            return;
        }

        alert(
            result.message ||
            "Session request updated successfully."
        );

        await loadSessionRequests();

    } catch (error) {

        console.error(error);

        alert(
            "Unable to connect to the API."
        );
    }
}


document.addEventListener(
    "DOMContentLoaded",
    function () {

        const skillSelect =
            document.getElementById("skillSelect");

        const mySkillsList =
            document.getElementById(
                "mySkillsList"
            );

        const addSkillButton =
            document.getElementById(
                "addSkillButton"
            );

        if (skillSelect) {
            loadSkills();
        }

        if (mySkillsList) {
            loadMySkills();
        }

        if (
            document.getElementById(
                "matchesList"
            )
        ) {
            loadMatches();
        }

        if (
            document.getElementById(
                "requestsList"
            )
        ) {
            loadSessionRequests();
        }

        if (addSkillButton) {

            addSkillButton.addEventListener(
                "click",
                addSkill
            );
        }
    }
);


async function loadUpcomingSessions() {

    const userId =
        sessionStorage.getItem("userId")

    const token =
        sessionStorage.getItem("token")

    if (!userId || !token) {
        return;
    }

    try {

        const response = await fetch(
            `https://localhost:7121/api/Sessions/upcoming/${userId}`,
            {
                method: "GET",
                headers: {
                    "Authorization":
                        `Bearer ${token}`
                }
            }
        );

        if (!response.ok) {

            throw new Error(
                "Unable to load sessions."
            );
        }

        const sessions =
            await response.json();

        const container =
            document.getElementById(
                "sessionsContainer"
            );

        if (sessions.length === 0) {

            container.innerHTML = `

                <div class="empty-state">

                    <div class="empty-state-icon">
                        📅
                    </div>

                    <h3>
                        No upcoming sessions
                    </h3>

                    <p>
                        Your scheduled sessions
                        will appear here.
                    </p>

                </div>
            `;

            return;
        }

        container.innerHTML = "";

        sessions.forEach(session => {

            const sessionDiv =
                document.createElement("div");

            sessionDiv.className =
                "session-card";

            const joined =
                session.currentUserJoinedAt;

            sessionDiv.innerHTML = `

                <div class="session-card-header">

                    <div>

                        <span class="session-label">
                            UPCOMING SESSION
                        </span>

                        <h3>
                            Skill Exchange Session
                        </h3>

                    </div>

                    <span class="session-status">
                        ${session.status}
                    </span>

                </div>

                <div class="session-details">

                    <div class="session-detail">

                        <span class="detail-label">
                            Date
                        </span>

                        <strong>
                            ${session.scheduledDate
                    .split("T")[0]}
                        </strong>

                    </div>

                    <div class="session-detail">

                        <span class="detail-label">
                            Time
                        </span>

                        <strong>
                            ${session.startTime}
                        </strong>

                    </div>

                    <div class="session-detail">

                        <span class="detail-label">
                            Duration
                        </span>

                        <strong>
                            ${session.durationMinutes}
                            minutes
                        </strong>

                    </div>

                </div>

                <div class="session-action">

    ${joined
                    ? `
            <div class="joined-message">
                ✓ You have joined
            </div>

            <button
                type="button"
                class="complete-session-button"
                onclick="completeSession(${session.sessionId})">
                Mark as Completed
            </button>
          `
                    : `
            <button
                type="button"
                class="join-session-button"
                onclick="joinSession(${session.sessionId})">
                Join Session
            </button>
          `
                }

</div>
            `;

            container.appendChild(sessionDiv);
        });

    } catch (error) {

        console.error(error);

        document.getElementById(
            "sessionsContainer"
        ).innerHTML =
            "<p>Unable to load upcoming sessions.</p>";
    }
}
async function joinSession(sessionId) {

    const token =
        sessionStorage.getItem("token");

    if (!token) {
        alert("Please login again.");
        return;
    }

    try {

        const response = await fetch(
            `https://localhost:7121/api/Sessions/${sessionId}/join`,
            {
                method: "POST",
                headers: {
                    "Authorization":
                        `Bearer ${token}`
                }
            }
        );

        const data =
            await response.json();

        if (!response.ok) {
            alert(
                data.message ||
                "Unable to join session."
            );
            return;
        }

        alert(
            "You joined the session successfully!"
        );

        // Reload upcoming sessions
        await loadUpcomingSessions();

    } catch (error) {

        console.error(
            "Join session error:",
            error
        );

        alert(
            "Unable to join the session."
        );
    }
}

async function joinSession(sessionId) {

    const token =
        sessionStorage.getItem("token");

    if (!token) {
        alert("Please login first.");
        return;
    }

    try {

        const response = await fetch(
            `/api/Sessions/${sessionId}/join`,
            {
                method: "POST",
                headers: {
                    "Authorization":
                        "Bearer " + token
                }
            }
        );

        const result =
            await response.json();

        if (!response.ok) {

            alert(
                result.message ||
                "Unable to join session."
            );

            return;
        }

        alert(
            result.message ||
            "Joined session successfully!"
        );

        await loadUpcomingSessions();

    } catch (error) {

        console.error(error);

        alert(
            "Unable to connect to the API."
        );
    }
}
async function completeSession(sessionId) {

    const token = sessionStorage.getItem("token");

    if (!token) {
        alert("Please login again.");
        return;
    }

    try {

        const response = await fetch(
            `https://localhost:7121/api/Sessions/${sessionId}/complete`,
            {
                method: "POST",
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            }
        );

        const data = await response.json();

        if (!response.ok) {
            alert(
                data.message ||
                "Unable to complete session."
            );
            return;
        }

        alert(
            "Session completion recorded successfully!"
        );

        await loadUpcomingSessions();

    } catch (error) {

        console.error(
            "Complete session error:",
            error
        );

        alert(
            "Unable to complete the session."
        );
    }
}

async function loadReviews(userId) {

    const token =
        sessionStorage.getItem("token");

    if (!token) {
        return;
    }

    try {

        const response = await fetch(
            `/api/Reviews/user/${userId}`,
            {
                headers: {
                    "Authorization":
                        "Bearer " + token
                }
            }
        );

        if (!response.ok) {

            throw new Error(
                "Failed to load reviews."
            );
        }

        const reviews =
            await response.json();

        const container =
            document.getElementById(
                "reviewsContainer"
            );

        if (
            !reviews ||
            reviews.length === 0
        ) {

            container.innerHTML = `

                <div class="empty-state">

                    <div class="empty-state-icon">
                        ★
                    </div>

                    <h3>
                        No reviews yet
                    </h3>

                    <p>
                        Reviews from your completed
                        sessions will appear here.
                    </p>

                </div>
            `;

            return;
        }

        container.innerHTML =
            reviews.map(review => `

                <div class="review-card">

                    <div class="review-card-header">

                        <div>

                            <span class="review-label">
                                SESSION REVIEW
                            </span>

                            <h3>
                                Your Feedback
                            </h3>

                        </div>

                        <div class="review-rating">
                            ${"⭐".repeat(review.rating)}
                        </div>

                    </div>

                    <div class="review-comment">

                        <span>
                            Comment
                        </span>

                        <p>
                            ${review.comment ||
                "No comment"}
                        </p>

                    </div>

                    <div class="review-date">

                        ${new Date(
                    review.createdAt
                ).toLocaleDateString()}

                    </div>

                </div>

            `).join("");

    } catch (error) {

        console.error(error);

        document.getElementById(
            "reviewsContainer"
        ).innerHTML =
            "<p>Unable to load reviews.</p>";
    }
}

// ================= FORGOT PASSWORD =================

const forgotPasswordLink = document.getElementById("forgotPasswordLink");
const forgotPasswordSection = document.getElementById("forgotPasswordSection");
const resetPasswordButton = document.getElementById("resetPasswordButton");
const backToLoginLink = document.getElementById("backToLoginLink");

let resetToken = null;

// Show Forgot Password section
if (forgotPasswordLink) {
    forgotPasswordLink.addEventListener("click", function (e) {
        e.preventDefault();

        document.getElementById("loginForm").style.display = "none";
        forgotPasswordSection.style.display = "block";
    });
}

// Back to Login
if (backToLoginLink) {
    backToLoginLink.addEventListener("click", function (e) {
        e.preventDefault();

        forgotPasswordSection.style.display = "none";
        document.getElementById("loginForm").style.display = "block";
    });
}

// Generate reset token
if (resetPasswordButton) {
    resetPasswordButton.addEventListener("click", async function () {

        const email = document.getElementById("resetEmail").value.trim();
        const newPassword = document.getElementById("newPassword").value;
        const confirmPassword = document.getElementById("confirmPassword").value;
        const message = document.getElementById("resetPasswordMessage");

        if (!email || !newPassword || !confirmPassword) {
            message.textContent = "Please fill in all fields.";
            return;
        }

        if (newPassword !== confirmPassword) {
            message.textContent = "Passwords do not match.";
            return;
        }

        try {
            // Step 1: Get reset token
            const forgotResponse = await fetch("/api/auth/forgot-password", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    email: email
                })
            });

            const forgotData = await forgotResponse.json();

            if (!forgotResponse.ok) {
                message.textContent =
                    forgotData.message || "No account found with this email.";
                return;
            }

            resetToken = forgotData.token;

            // Step 2: Reset password
            const resetResponse = await fetch("/api/auth/reset-password", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    token: resetToken,
                    newPassword: newPassword
                })
            });

            const resetData = await resetResponse.text();

            if (!resetResponse.ok) {
                message.textContent = resetData || "Password reset failed.";
                return;
            }

            message.textContent = "Password reset successfully!";

            setTimeout(() => {
                forgotPasswordSection.style.display = "none";
                document.getElementById("loginForm").style.display = "block";
            }, 1500);

        } catch (error) {
            console.error(error);
            message.textContent = "Something went wrong. Please try again.";
        }
    });
}

async function submitReview() {

    const token = sessionStorage.getItem("token");
    const userId = sessionStorage.getItem("userId");

    const sessionId =
        document.getElementById("reviewSessionId").value;

    const reviewedUserId =
        document.getElementById("reviewedUserId").value;

    const rating =
        document.getElementById("reviewRating").value;

    const comment =
        document.getElementById("reviewComment").value.trim();

    const message =
        document.getElementById("reviewMessage");


    if (!token || !userId) {

        message.textContent =
            "Please login again.";

        return;
    }


    if (!sessionId || !reviewedUserId) {

        message.textContent =
            "Session information is missing.";

        return;
    }


    if (!rating) {

        message.textContent =
            "Please select a rating.";

        return;
    }


    if (!comment) {

        message.textContent =
            "Please enter your review.";

        return;
    }


    try {

        const response = await fetch(
            "/api/Reviews",
            {
                method: "POST",

                headers: {
                    "Content-Type": "application/json",
                    "Authorization":
                        "Bearer " + token
                },

                body: JSON.stringify({

                    sessionId:
                        Number(sessionId),

                    reviewedUserId:
                        Number(reviewedUserId),

                    rating:
                        Number(rating),

                    comment:
                        comment
                })
            }
        );


        const resultText =
            await response.text();

        let result;

        try {

            result =
                JSON.parse(resultText);

        } catch {

            result =
                resultText;
        }


        if (!response.ok) {

            message.textContent =
                typeof result === "string"
                    ? result
                    : result.message ||
                    "Unable to submit review.";

            return;
        }


        message.textContent =
            "Review submitted successfully!";


        document.getElementById(
            "reviewRating"
        ).value = "";


        document.getElementById(
            "reviewComment"
        ).value = "";


        // Hide review form after successful submission
        document.getElementById(
            "sessionReviewSection"
        ).style.display = "none";


        // Refresh received reviews
        await loadReviews(userId);

    }
    catch (error) {

        console.error(
            "Submit review error:",
            error
        );

        message.textContent =
            "Unable to submit review. Please try again.";
    }
}



async function loadCompletedSessions() {
    const userId = sessionStorage.getItem("userId");
    const token = sessionStorage.getItem("token");

    if (!userId || !token) return;

    try {
        const response = await fetch(
            `https://localhost:7121/api/Sessions/completed/${userId}`,
            {
                method: "GET",
                headers: {
                    "Authorization": "Bearer " + token
                }
            }
        );

        if (!response.ok) {
            console.error("Unable to load completed sessions.");
            return;
        }

        const sessions = await response.json();

        console.log("Completed sessions:", sessions);

        if (!sessions || sessions.length === 0) {
            document.getElementById("sessionReviewSection").style.display = "none";
            return;
        }

        const currentUserId = Number(userId);

        // Find the first completed session that the user has NOT reviewed
        let sessionToReview = null;

        for (const session of sessions) {

            const reviewResponse = await fetch(
                `/api/Reviews/session/${session.sessionId}/mine`,
                {
                    method: "GET",
                    headers: {
                        "Authorization": "Bearer " + token
                    }
                }
            );

            if (!reviewResponse.ok) {
                console.error(
                    "Unable to check review status for session:",
                    session.sessionId
                );
                continue;
            }

            const reviewStatus = await reviewResponse.json();

            console.log(
                "Review status for session",
                session.sessionId,
                ":",
                reviewStatus
            );

            if (!reviewStatus.exists) {
                sessionToReview = session;
                break;
            }
        }

        // All completed sessions have already been reviewed
        if (!sessionToReview) {
            document.getElementById("sessionReviewSection").style.display = "none";
            return;
        }

        let reviewedUserId;

        if (Number(sessionToReview.requesterUserId) === currentUserId) {
            reviewedUserId = sessionToReview.receiverUserId;
        } else {
            reviewedUserId = sessionToReview.requesterUserId;
        }

        document.getElementById("reviewSessionId").value =
            sessionToReview.sessionId;

        document.getElementById("reviewedUserId").value =
            reviewedUserId;

        document.getElementById("sessionReviewSection").style.display =
            "block";

    } catch (error) {
        console.error("Load completed sessions error:", error);
    }
}
// ================= DASHBOARD =================

async function loadDashboard() {

    const userId = sessionStorage.getItem("userId");
    const token = sessionStorage.getItem("token");

    if (!userId || !token) {
        return;
    }

    const headers = {
        "Authorization": "Bearer " + token
    };

    try {

        // My Skills
        const skillsResponse = await fetch(
            "/api/UserSkills",
            {
                headers: headers
            }
        );

        if (skillsResponse.ok) {

            const skills = await skillsResponse.json();

            document.getElementById("skillsCount").textContent =
                skills.length;

            const skillsContainer =
                document.getElementById("mySkills");

            if (skillsContainer && skills.length > 0) {

                skillsContainer.innerHTML = skills.map(skill => `
                    <div class="skill-card">
                        <h3>${skill.skillName}</h3>
                        <span>${skill.skillType}</span>
                        <p>${skill.skillLevel || ""}</p>
                    </div>
                `).join("");

            }
        }


        // Upcoming Sessions
        const sessionsResponse = await fetch(
            `https://localhost:7121/api/Sessions/upcoming/${userId}`,
            {
                headers: headers
            }
        );

        if (sessionsResponse.ok) {

            const sessions = await sessionsResponse.json();

            document.getElementById("sessionsCount").textContent =
                sessions.length;

            const sessionsContainer =
                document.getElementById("upcomingSessions");

            if (sessionsContainer && sessions.length > 0) {

                sessionsContainer.innerHTML = sessions.map(session => `
                    <div class="session-card">

                        <h3>Skill Exchange Session</h3>

                        <p>
                            <strong>Date:</strong>
                            ${session.scheduledDate.split("T")[0]}
                        </p>

                        <p>
                            <strong>Time:</strong>
                            ${session.startTime}
                        </p>

                        <p>
                            <strong>Duration:</strong>
                            ${session.durationMinutes} minutes
                        </p>

                        <span class="session-status">
                            ${session.status}
                        </span>

                    </div>
                `).join("");

            }
        }


        // Session Requests
        const requestsResponse = await fetch(
            "/api/SessionRequests",
            {
                headers: headers
            }
        );

        if (requestsResponse.ok) {

            const requests = await requestsResponse.json();

            document.getElementById("requestsCount").textContent =
                requests.length;

            const requestsContainer =
                document.getElementById("sessionRequests");

            if (requestsContainer && requests.length > 0) {

                requestsContainer.innerHTML = requests.map(request => `
                    <div class="request-card">

                        <h3>${request.requesterName}</h3>

                        <p>
                            Skill: ${request.skillName}
                        </p>

                        <span class="status-badge">
                            ${request.status}
                        </span>

                    </div>
                `).join("");

            }
        }


        // Reviews
        const reviewsResponse = await fetch(
            `/api/Reviews/user/${userId}`,
            {
                headers: headers
            }
        );

        if (reviewsResponse.ok) {

            const reviews = await reviewsResponse.json();

            document.getElementById("reviewsCount").textContent =
                reviews.length;
        }


        // User name
        const userName =
            sessionStorage.getItem("name");

        if (userName) {

            document.getElementById(
                "dashboardUserName"
            ).textContent = userName;

            document.getElementById(
                "userAvatar"
            ).textContent =
                userName.charAt(0).toUpperCase();
        }

    }
    catch (error) {

        console.error(
            "Dashboard loading error:",
            error
        );
    }
}


// Load dashboard only when Dashboard elements exist
if (document.getElementById("skillsCount")) {
    loadDashboard();
}

function logout() {

    sessionStorage.removeItem("token");
    sessionStorage.removeItem("userId");
    sessionStorage.removeItem("name");
    sessionStorage.removeItem("email");

    window.location.href = "login.html";
}
loadUpcomingSessions();
loadCompletedSessions();