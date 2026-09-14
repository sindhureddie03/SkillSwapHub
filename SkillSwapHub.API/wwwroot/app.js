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
                localStorage.setItem("token", data.token);
                localStorage.setItem("userId", data.userId);
                localStorage.setItem("name", data.name);
                localStorage.setItem("email", data.email);

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

    const token = localStorage.getItem("token");
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

                localStorage.removeItem("token");
                localStorage.removeItem("userId");
                localStorage.removeItem("name");
                localStorage.removeItem("email");

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
                localStorage.getItem("token");

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
        localStorage.getItem("token");

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
        localStorage.getItem("token");

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
        localStorage.getItem("token");

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
        localStorage.getItem("token");

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
        localStorage.getItem("token");

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
        localStorage.getItem("token");

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
        localStorage.getItem("token");

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
        localStorage.getItem("token");

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
        localStorage.getItem("token");

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
        localStorage.getItem("token");

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
        localStorage.getItem("userId");

    const token =
        localStorage.getItem("token");

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
                session.learnerJoinedAt;

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
                              `
                    : `
                                <button
                                    type="button"
                                    class="join-session-button"
                                    onclick="joinSession(
                                        ${session.sessionId}
                                    )">
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
        localStorage.getItem("token");

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


async function loadReviews(userId) {

    const token =
        localStorage.getItem("token");

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


loadUpcomingSessions();